using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Graphics_New
{
    public class Record
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Signal> Signals { get; set; }
        public int RecordNumber = 1;
        public long Pk_Record;
        public void AddSignal(int index, string signalName, string Col,bool IsDetector, string unit)
         {
            Signals.Add(new Signal (index, signalName, Col, IsDetector, unit)); // Default signal initialization
         }

        public List<Signal> GetSignals()
        {
            return Signals;
        }

        public Record(int? RecNumLoad=null , int? RunNumAttached=null)
        {
            Signals = new List<Signal>();

            if(RecNumLoad.HasValue)
            {
                XML_Manager.LoadSignalsConf(this);
                RecordNumber = RecNumLoad.Value;
            }

            if (RunNumAttached.HasValue)
            {
                this.CreationDate = DateTime.Now;
                this.EndDate = DateTime.Now;
                XML_Manager.LoadSignalsConf(this);
                SQLite.InsertSignalFromRec(this);
                if (SQLite.GetLastRecordOfRun(RunNumAttached.Value) > 0)
                {
                    RecordNumber = SQLite.GetLastRecordOfRun(RunNumAttached.Value) + 1;

                }
                Data.CurrentRecord = RecordNumber;
                Pk_Record = SQLite.InsertRecord(RunNumAttached.Value, RecordNumber,"No Rec title" , "No description set");
            }


        }

    }

}
