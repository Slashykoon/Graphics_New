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
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Signal> Signals { get; set; }
        public int RecordNumber = 1;

        public void AddSignal(int index, string signalName, string Col,bool IsDetector)
         {
            Signals.Add(new Signal (index, signalName, Col, IsDetector)); // Default signal initialization
         }

        public List<Signal> GetSignals()
        {
            return Signals;
        }

        public Record(int RunNumAttached)
        {
            Signals = new List<Signal>();
            this.CreationDate = DateTime.Now;
            this.EndDate = DateTime.Now;
            XML_Manager.LoadSignalsConf(this);
            SQLite.InsertSignalFromRec(this);
            if (SQLite.GetLastRecordOfRun(RunNumAttached)>0)
            {
                RecordNumber = SQLite.GetLastRecordOfRun(RunNumAttached);
                
            }
            Data.CurrentRecord = RecordNumber;
            SQLite.InsertRecord(RunNumAttached, RecordNumber, "No description set");
        }
    }

}
