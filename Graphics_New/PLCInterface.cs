using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Xml;
using OpenTK.Graphics.OpenGL;
using ScottPlot.Colormaps;
using ScottPlot.WinForms;
using Snap7;
using static ScottPlot.Generate;
using static System.Windows.Forms.AxHost;
using static SkiaSharp.HarfBuzz.SKShaper;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Graphics_New
{
    public enum PLCBufferCode : int
    {
        NewRecord = 1,
        NewCollector = 2,
        NewReference = 4,
        Stopped = 16
    }
    enum ParserState
    {
        WaitingForNewRecord,
        Recording,
        Stopped
    }
    public class GraphicsDB
    {

        public int spare0;
        public int iWRITEINDEX;
        public bool bCLININJCMDMEM;
        public bool bNEWCYC;
        public int[] iCURVLV = new int[8];
        public int iCURREF;

        public uint[] Header = new uint[100];
        public Single[] Values = new Single[2048];
        public int[] ValuesDint = new int[2048];
        public uint kMaxValueIndex => Header[0];
        public uint kWriteIndexIndex => Header[1];
        public uint kWriteEluDurationTime => Header[2];
        public uint kWriteCycleTime => Header[3];
        public uint kWriteCycleNumber => Header[4];
        public uint kCurrentDetector => Header[5];
        public uint kCurrentSignalNb => Header[6];
        public uint kSamplingPeriod => Header[7];
        public uint kSmoothingFactor => Header[8];
        public uint kNegativeSlope => Header[9];
        public uint kPositiveSlope => Header[10];
        public uint kSlopeSmoothing => Header[11];
        public uint kMask1Start => Header[12];
        public uint kMask1Stop => Header[13];
        public uint kMask2Start => Header[14];
        public uint kMask2Stop => Header[15];
        public uint kAbsTime => Header[16];
        public uint kRelativeTime => Header[17];
        public uint kAbsSignal => Header[18];
        public uint kRelativeSignal => Header[19];
        public uint kLearningCycle => Header[20];

        public Dictionary<int, List<Single>> ArrByteFormatted = new Dictionary<int, List<Single>>(); //index trame, index value , value

    }

    public class PLCInterface
    {
        private static S7Client client = new S7Client();
        private Thread readThread;
        private GraphicsDB db = new GraphicsDB();
        private bool isRunning = false;
        private object dbLock = new object();
        private int Curr_idx;
        private int Old_idx=1;
        private bool _NewRecordTriggered;
        public event Action OnNewRecord;
        public int lastIdxofArrayCopied = 0;
        public bool bRecordData = false;
        private int lastNewRecordIndex = -1;
        private int lastNewCollectIndex = -1;
        private int lastStopped = -1;
        string exeDirectory;
        string filePath;

        public bool NewRecordTriggered
        {
            get => _NewRecordTriggered;
            set
            {
                if(!_NewRecordTriggered && value)
                {
                    OnNewRecord?.Invoke();
                }
            }
        }

        public GraphicsDB GraphicsDB { get; private set; }

        private ParserState currentState = ParserState.WaitingForNewRecord;
        private int lastProcessedIndex = -1;
        private int lastEventIndex = 1;
        private System.Threading.Timer timer;

        public PLCInterface()
        {
            timer = new System.Threading.Timer(
                                                callback: ConnectionToPLC,
                                                state: null,
                                                dueTime: Timeout.Infinite,  // do NOT start immediately
                                                period: Timeout.Infinite    // do NOT repeat until started
                                            );
            StartTimerConnection();

    
        }

        public void ConnectionToPLC(object state)
        {
            Tools.LogToFile("Trying connection to PLC");
            int result = client.ConnectTo("192.168.0.1", 0, 1);

            if (result != 0)
            {
                Tools.LogToFile("Connection failed: " + client.ErrorText(result));
            }
            else
            {
                short Timeout = 5000;
                client.SetParam(S7Consts.p_i32_RecvTimeout, ref Timeout);
                Tools.LogToFile("Connection OK");
                StopTimerConnection();
                isRunning = true;
                readThread = new Thread(ReadLoop)
                {
                    IsBackground = true,
                    Name = "PLCReadThread" // Name for easier identification
                };
                readThread.Start();
                Tools.LogToFile("Thread PLC read loop started");

            }
        }

        public  void StopTimerConnection()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
            //timer?.Dispose();
        }
        public  void StartTimerConnection()
        {
            timer.Change(0, 2000);  // Restart immediately, tick every 1000 ms
        }


        public void StopReadingLoop()
        {
            isRunning = false;
            readThread?.Join(2000); // Wait up to 2 seconds for thread to exit
            if (readThread?.IsAlive ?? false)
            {
                Tools.LogToFile("Forcefully aborting PLC read thread");
                readThread.Abort(); // Last resort
            }
            client.Disconnect();
            Tools.LogToFile("Reading in PLC is stopped");
        }

        private void ReadLoop()
        {
            Tools.LogToFile("ReadLoop started on thread ");
            while (isRunning)
            {
                try
                {
                    var db = ReadGraphicsDB();
                    if (db != null)
                    {
                        lock (dbLock)
                        {
                            GraphicsDB = db;
                        }
                    }
                    //Tools.LogToFile("Cyclic reading done on thread: ");
                }
                catch (Exception ex)
                {
                    Tools.LogToFile($"ReadLoop error: {ex.Message} on thread: " + Thread.CurrentThread.ManagedThreadId);
                    if (client != null && !client.Connected())
                    {
                        Tools.LogToFile("Attempting to reconnect...");
                        client.Disconnect();
                        int result = client.ConnectTo("192.168.0.1", 0, 1);
                        if (result == 0 && client.Connected())
                        {
                            Tools.LogToFile("Reconnection successful on thread: " + Thread.CurrentThread.ManagedThreadId);
                        }
                        else
                        {
                            Tools.LogToFile("Reconnection failed: " + client.ErrorText(result));
                        }
                    }
                }
                Thread.Sleep(500);
            }
            Tools.LogToFile("ReadLoop ended on thread: " + Thread.CurrentThread.ManagedThreadId);
        }


        public GraphicsDB ReadGraphicsDB()
        {
            int dbNumber = 101;
            int size = 24 + 2048 * 4;
            byte[] buffer = new byte[size];

            int result = client.DBRead(dbNumber, 0, size, buffer);
            if (result != 0)
            {
                Tools.LogToFile("DB read failed: " + client.ErrorText(result));
                return null;
            }

            return ParseGraphicsDB(buffer);
        }

         GraphicsDB ParseGraphicsDB(byte[] buffer)
        {
            // Get the directory where the executable is located
            //exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Combine the executable directory with the log file name
            //filePath = Path.Combine(exeDirectory, "datas.bin");



            //parsing du buffer de données 
            db.spare0 = BitConverter.ToInt16(buffer, 0);
            db.iWRITEINDEX = S7.GetIntAt(buffer, 2);
            db.bCLININJCMDMEM = (buffer[4] & 0x01) != 0;
            db.bNEWCYC = (buffer[4] & 0x02) != 0;

            for (int i = 0; i < 8; i++)
            {
                db.iCURVLV[i] = BitConverter.ToInt16(buffer, 6 + i * 2);
            }

            db.iCURREF = BitConverter.ToInt16(buffer, 22);

            int offsetH = 0;
            int offsetV = 0;
            int LengthBefore_ByteArray = 24;
            int FirstByte_BtwHeadAndValue = 22;
            int OffsetFirstValue_ByteArray = LengthBefore_ByteArray + (FirstByte_BtwHeadAndValue * 4);
            int inc = 0;

            for (int i = 0; i < FirstByte_BtwHeadAndValue; i++)
            {
                offsetH = LengthBefore_ByteArray + i * 4;
                db.Header[i] = S7.GetDWordAt(buffer, offsetH);
            }

            for (int i = 0; i < (2048 - offsetH); i++)
            {
                offsetV = OffsetFirstValue_ByteArray + i * 4;
                db.Values[i] = S7.GetRealAt(buffer, offsetV);
                db.ValuesDint[i] = S7.GetDIntAt(buffer, offsetV);
            }
            Tools.LogToFile("write index plc : "+ db.iWRITEINDEX);

            //Check code function 
            if(db.iWRITEINDEX <=1)
            {
                lastEventIndex = 1;
            }
            for (int i = lastEventIndex; i <= db.kMaxValueIndex; i++)
            {
                byte[] bytes = BitConverter.GetBytes(db.ValuesDint[((i - 1) * (db.kCurrentSignalNb+1))]);
                Tools.LogToFile("Bytes de code fonction ("+ lastEventIndex.ToString()+ ") : " + bytes[3] + " " + bytes[2] + " " + bytes[1] + " " + bytes[0]);
                if (CheckCode(bytes,i))
                {
                    lastEventIndex = i+1;
                    if(lastEventIndex > db.kMaxValueIndex)
                    {
                        lastEventIndex = 1;
                    }
                }
            }

            if (bRecordData)
            {
                //recopie des dernières données à jour du buffer
                if (Old_idx < db.iWRITEINDEX)
                {
                    for (int i = Old_idx+1; i <= db.iWRITEINDEX; i++)
                    {
                        List<Single> tmp = new List<Single>();

                        for (int j = 1; j <= db.kCurrentSignalNb; j++) // j = 1 car on veut pas le code fonction
                        {
                            tmp.Add(db.Values[(i + (i - 1) * db.kCurrentSignalNb) + j]);
                        }

                        LogValues(tmp);

                        db.ArrByteFormatted.Add(Curr_idx, tmp);
                        Curr_idx++;

                        AppendToBinaryFile(Data.CurrentRun, Data.CurrentRecord, db.ArrByteFormatted);
                        AppendPLCDataToSignals(); 
                    }
                    Old_idx = db.iWRITEINDEX;
                }
                else if (Old_idx > db.iWRITEINDEX)
                {
                    //Tools.LogToFile("Add from " + Old_idx + " to " + db.kMaxValueIndex);
                    for (int i = Old_idx+1; i <= db.kMaxValueIndex; i++)
                    {
                        List<Single> tmp = new List<Single>();

                        for (int j = 1; j <= db.kCurrentSignalNb; j++)
                        {
                            tmp.Add(db.Values[(i + (i - 1) * db.kCurrentSignalNb) + j]);
                        }

                        LogValues(tmp);

                        db.ArrByteFormatted.Add(Curr_idx, tmp);
                        Curr_idx++;


                        AppendToBinaryFile(Data.CurrentRun, Data.CurrentRecord, db.ArrByteFormatted);
                        AppendPLCDataToSignals(); 
                    }
                    Old_idx = 0; // voir si mieux possible recup autre donnee
                }
            }
            return db;
        }


        public void AppendPLCDataToSignals()
        {
            if (db == null || db.ArrByteFormatted.Count == 0)
                return;

            List<Signal> signals = Data.GetSignals(Data.CurrentRun, Data.CurrentRecord);
            int newCount = db.ArrByteFormatted.Count;

            for (int i = lastIdxofArrayCopied; i < newCount; i++)
            {
                List<Single> newValues = db.ArrByteFormatted[i];
                for (int j = 0; j < signals.Count; j++)
                {
                    Signal s = signals[j];
                  
                    s.CurveLogger.Add((i * 500.0f) / 1000.0f, newValues[j]);
                }
            }
            lastIdxofArrayCopied = newCount;
        }

        public void AppendAllPLCDataToSignals(Dictionary<int, List<Single>> dictAllDataLoaded,int RunLoaded, int RecLoaded)
        {
            if (dictAllDataLoaded.Count == 0)
                return;
            Data.AddNewRun(RunLoaded);
            Data.dRuns[RunLoaded].AttachNewRecord(RecLoaded);
            List<Signal> signals = Data.GetSignals(RunLoaded, RecLoaded);
            int newCount = dictAllDataLoaded.Count;

            for (int i = 0; i < newCount; i++)
            {
                List<Single> newValues = dictAllDataLoaded[i];
                for (int j = 0; j < signals.Count; j++)
                {
                    Signal s = signals[j];

                    //s.YPoints.Add(newValues[j]);
                    //s.XPoints.Add((i*500.0f)/1000.0f);
                    s.CurveLogger.Add((i * 500.0f) / 1000.0f, newValues[j]);
                }
            }
            Tools.LogToFile("AppendAllPLCDataToSignals");
        }




        void AppendToBinaryFile(int runNumber, int recordNumber, Dictionary<int, List<Single>> newData)
        {
            string filePath = Tools.GetBinaryFilePath(runNumber, recordNumber);
            using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            using var writer = new BinaryWriter(fs);
            int curr_key = newData.Keys.Max();
            writer.Write(curr_key); // Write the key
            writer.Write(newData[curr_key].Count); // Write the number of floats
            foreach (var val in newData[curr_key])
            {
                writer.Write(val); // Write the floats
            }
        }

        Dictionary<int, List<Single>> ReadFromBinaryFile(int runNumber, int recordNumber)
        {
            string filePath = Tools.GetBinaryFilePath(runNumber, recordNumber);
            var result = new Dictionary<int, List<Single>>();

            if (!File.Exists(filePath))
            {
                Tools.LogToFile($"Binary file {filePath} does not exist.");
                return result;
            }

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs);

            while (fs.Position < fs.Length)
            {
                int key = reader.ReadInt32();
                int count = reader.ReadInt32();
                var list = new List<Single>();

                for (int i = 0; i < count; i++)
                {
                    list.Add(reader.ReadSingle());
                }

                if (!result.ContainsKey(key))
                    result[key] = new List<Single>();

                result[key].AddRange(list);
            }
            return result;
        }



        public bool CheckCode(byte[] b, int idx)
        {
            byte code = b[0];
            bool EventOccured = false;
            switch (currentState)
            {
                case ParserState.WaitingForNewRecord:
                    if ((code & (byte)PLCBufferCode.NewRecord) != 0 && idx != lastNewRecordIndex)
                    {
                        Tools.LogToFile("*** Nouveau record de démarrage! *** " + idx + " " + currentState.ToString());
                        lastNewRecordIndex = idx;
                        EventOccured = true;
                        db.ArrByteFormatted.Clear();
                        Curr_idx = 0;
                        lastIdxofArrayCopied = 0; // Reset last copied index
                        bRecordData = true;
                        NewRecordTriggered = true;
                        currentState = ParserState.Recording;
                    }
                    break;

                case ParserState.Recording:
                    if ((code & (byte)PLCBufferCode.NewCollector) != 0 && idx != lastNewCollectIndex)
                    {
                        Tools.LogToFile("*** Nouvelle collection ! *** " + currentState.ToString());
                        lastNewCollectIndex = idx;
                        EventOccured = true;
                    }
                    else if ((code & (byte)PLCBufferCode.NewReference) != 0 && idx != lastProcessedIndex)
                    {
                        Tools.LogToFile("*** Nouvelle reference !  *** " + currentState.ToString());
                        lastProcessedIndex = idx;
                        EventOccured = true;
                    }
                    else if ((code & (byte)PLCBufferCode.Stopped) != 0 && idx != lastStopped)
                    {
                        Tools.LogToFile("*** Stoppé ! *** " + currentState.ToString());
                        lastStopped = idx;
                        EventOccured = true;
                        currentState = ParserState.Stopped;
                        bRecordData = false;
                    }
                    else if ((code & (byte)PLCBufferCode.NewRecord) != 0 && idx != lastNewRecordIndex)
                    {
                        Tools.LogToFile("*** Nouveau Record suite ! *** " + currentState.ToString());
                        lastNewRecordIndex = idx;
                        EventOccured = true;
                        db.ArrByteFormatted.Clear();
                        Curr_idx = 0;
                        lastIdxofArrayCopied = 0; // Reset last copied index
                        bRecordData = true;
                        NewRecordTriggered = true;
                        currentState = ParserState.Recording;
                        //Dictionary<int, List<Single>> ReadedDict = ReadFromBinaryFile(filePath);
                    }
                    break;

                case ParserState.Stopped:
                    if ((code & (byte)PLCBufferCode.NewRecord) != 0 && idx != lastNewRecordIndex)
                    {
                        Tools.LogToFile("*** Nouveau record ! *** " + idx + " "+currentState.ToString());
                        lastNewRecordIndex = idx;
                        EventOccured = true;
                        db.ArrByteFormatted.Clear();
                        Curr_idx = 0;
                        lastIdxofArrayCopied = 0; // Reset last copied index
                        bRecordData = true;
                        NewRecordTriggered = true;
                        //Dictionary<int, List<Single>> ReadedDict = ReadFromBinaryFile(filePath);
                        currentState = ParserState.Recording;
                    }
                    // Ignore everything else
                    break;
            }

            return EventOccured;

        }
        public void LogValues(List<float> _tmp)
        {
            string tempstr = "";
            foreach (Single T in _tmp)
            {
                tempstr = tempstr + " " + T.ToString();
            }
            Tools.LogToFile("Valeurs : " + tempstr);
        }

    }
}
