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

        public bool AttachNewRecord(int? index = null)
        {
            try
            {
                if (index.HasValue) // Un numéro est donné, on devine qu'on charge un record
                {
                    dRecords[index.Value] = new Record(index.Value);
                }
                else
                {
                    dRecords[SQLite.GetLastRecordOfRun(RunNumber) + 1] = new Record(null, RunNumber);
                }
                    
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
            SQLite.InsertRun(RunNumber,"No title", "No description set");
            
        }
    }
}
