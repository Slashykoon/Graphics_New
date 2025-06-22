using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Xml;
using OpenTK.Graphics.OpenGL;
using ScottPlot.Colormaps;
using Snap7;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Graphics_New
{
    public enum PLCBufferCode : int
    {
        NewRecord = 1,
        NewCollector = 2,
        NewReference = 4,
        Stopped = 16
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
        private S7Client client = new S7Client();
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
        private bool bdebug = true;

        public GraphicsDB GraphicsDB { get; private set; }

        public bool StartReadingLoop(string ip = "192.168.0.1", int rack = 0, int slot = 1)
        {
            int result = client.ConnectTo(ip, rack, slot);
            if (result != 0)
            {
                Tools.LogToFile("Connection failed: " + client.ErrorText(result));
                return false;
            }

            isRunning = true;
            readThread = new Thread(ReadLoop)
            {
                IsBackground = true,
                Name = "PLCReadThread" // Name for easier identification
            };
            readThread.Start();
            Tools.LogToFile("Thread PLC read started on thread: " + readThread.ManagedThreadId);
            return true;
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
            Tools.LogToFile("ReadLoop started on thread: " + Thread.CurrentThread.ManagedThreadId);
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
                    Tools.LogToFile("Cyclic reading done on thread: " + Thread.CurrentThread.ManagedThreadId);
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

        private GraphicsDB ParseGraphicsDB(byte[] buffer)
        {


            // Get the directory where the executable is located
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Combine the executable directory with the log file name
            string filePath = Path.Combine(exeDirectory, "datas.bin");

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

            for (int i = 1; i <= db.kMaxValueIndex; i++)
            {
                byte[] bytes = BitConverter.GetBytes(db.ValuesDint[((i - 1) * (db.kCurrentSignalNb+1))]);
                Tools.LogToFile("Bytes de code fonction : " + bytes[3] + " " + bytes[2] + " " + bytes[1] + " "  + bytes[0] );
                CheckCode(bytes,i);
            }
            if (bRecordData)
            {
                //recopie des dernières données à jour du buffer
                if (Old_idx < db.iWRITEINDEX)
                {
                    //Tools.LogToFile("Add from " + Old_idx + " to " + db.iWRITEINDEX);
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

                        AppendToBinaryFile(filePath, db.ArrByteFormatted);
                        AppendAllPLCDataToSignals(); //test
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


                        AppendToBinaryFile(filePath, db.ArrByteFormatted);
                        AppendAllPLCDataToSignals(); //test
                    }
                    Old_idx = 0; // voir si mieux possible recup autre donnee
                }
                //Dictionary<int, List<Single>> ReadedDict = ReadFromBinaryFile(filePath);
            }
            return db;
        }


        public void AppendAllPLCDataToSignals()
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
                    
                    //s.YPoints.Add(newValues[j]);
                    //s.XPoints.Add((i*500.0f)/1000.0f);
                    s.CurveLogger.Add((i * 500.0f) / 1000.0f, newValues[j]);
                }
            }
            lastIdxofArrayCopied = newCount;
        }
            



        void AppendToBinaryFile(string path, Dictionary<int, List<Single>> newData)
        {
            using var fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            using var writer = new BinaryWriter(fs);
            int curr_key;
            curr_key = newData.Keys.Max();
            writer.Write(curr_key);     // Écrit la clé
            writer.Write(newData[curr_key].Count);      // Écrit le nombre de floats

            foreach (var val in newData[curr_key])
            {
                writer.Write(val);           // Écrit les floats
            }
        }

        Dictionary<int, List<Single>> ReadFromBinaryFile(string path)
        {
            var result = new Dictionary<int, List<Single>>();

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
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

        public void CheckCode(byte[] b,int idx)
        {
            if ((b[0] & (byte)PLCBufferCode.NewRecord) != 0)
            {
                Tools.LogToFile("*** Nouveau record ! *** ");
                //Old_idx=idx;
                bRecordData = true;
                NewRecordTriggered = true;
                
            }
            if ((b[0] & (byte)PLCBufferCode.NewCollector) != 0)
            {
                Tools.LogToFile("*** Nouvelle collection ! *** ");
            }
            if ((b[0] & (byte)PLCBufferCode.NewReference) != 0)
            {
                Tools.LogToFile("*** Nouvelle reference ! *** ");
            }
            if ((b[0] & (byte)PLCBufferCode.Stopped) != 0)
            {
                Tools.LogToFile("*** Stoppé ! *** ");
            }
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
        public Dictionary<int, List<Single>> GetDictValues()
        {
            return db.ArrByteFormatted;
        }
    }
}
