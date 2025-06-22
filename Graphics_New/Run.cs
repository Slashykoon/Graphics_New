using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_New
{
    public class Run
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CycleNumber { get; set; }
        public Dictionary<int, Record> dRecords { get; set; }

        public int RunNumber = 0;

        public bool AttachNewRecord()
        {
            try
            {
                dRecords[SQLite.GetLastRecordOfRun(RunNumber) + 1] = new Record(RunNumber);
                return true; 
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Run()
        {
            dRecords = new Dictionary<int, Record>();
            RunNumber = SQLite.GetLastRunNumber() + 1;
            SQLite.InsertRun(RunNumber, "No description set");
            AttachNewRecord();
        }
    }
}
