using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Graphics_New
{

    public static class SQLite
    {
        private static readonly string connectionString = "Data Source=data.db";

        static SQLite()
        {
            // Ensure the database is initialized when the class is first accessed
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                PRAGMA journal_mode=WAL;
                PRAGMA synchronous=NORMAL;

                CREATE TABLE IF NOT EXISTS Signals (
                    Pk_Signal INTEGER PRIMARY KEY AUTOINCREMENT,
                    Position INTEGER,
                    Name TEXT NOT NULL
                );

                -- Runs table
                CREATE TABLE IF NOT EXISTS Runs( 
                    Pk_Run INTEGER PRIMARY KEY AUTOINCREMENT,             
                    RunNumber INTEGER, 
                    RunName TEXT NOT NULL,
                    RunDescription TEXT
                );

                -- Join table for many-to-many relationship between Runs and Signals
                CREATE TABLE IF NOT EXISTS RunSignals (
                    Fk_Run INTEGER NOT NULL,
                    Fk_Signal INTEGER NOT NULL,
                    PRIMARY KEY (Fk_Run, Fk_Signal),
                    FOREIGN KEY (Fk_Run) REFERENCES Runs(Pk_Run) ON DELETE CASCADE,
                    FOREIGN KEY (Fk_Signal) REFERENCES Signals(Pk_Signal) ON DELETE CASCADE
                );

                -- Records table
                CREATE TABLE IF NOT EXISTS Records( 
                    Pk_Record INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Fk_Run INTEGER,
                    RecordNumber INTEGER, 
                    RecordName TEXT NOT NULL,
                    RecordDescription TEXT,
                    RecordData BLOB,
                    FOREIGN KEY (Fk_Run) REFERENCES Runs(Pk_Run) ON DELETE CASCADE
                );
                ";

                command.ExecuteNonQuery();
            }
        }

        public static long InsertSignal(string name, int position)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Check if signal with the same name and position already exists
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = @"
                SELECT Pk_Signal
                FROM Signals
                WHERE Name = @name AND Position = @position
                LIMIT 1;";

            checkCommand.Parameters.AddWithValue("@name", name);
            checkCommand.Parameters.AddWithValue("@position", position);
            var existingPk = checkCommand.ExecuteScalar();

            if (existingPk != null && existingPk != DBNull.Value)
            {
                return (long)existingPk; // Return existing Pk_Signal if name and position match
            }

            // If no exact match, insert new signal (allowing duplicate names with different positions)
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO Signals (Name, Position)
                VALUES (@name, @position);
                SELECT last_insert_rowid();";

            insertCommand.Parameters.AddWithValue("@name", name);
            insertCommand.Parameters.AddWithValue("@position", position);

            return (long)insertCommand.ExecuteScalar(); // Returns the new Pk_Signal
        }

        public static void InsertSignalFromRec(Record rec)
        {
            foreach (Signal sig in rec.GetSignals()) 
            {
                InsertSignal(sig.Name, sig.Index);
            }

        }

        public static long InsertRun(int runNumber, string runName, string description)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Runs (RunNumber,RunName, RunDescription)
            VALUES (@number,@name, @desc);
            SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@number", runNumber);
            command.Parameters.AddWithValue("@name", runName);
            command.Parameters.AddWithValue("@desc", description);

            return (long)command.ExecuteScalar(); // returns the new Pk_Run
        }

        public static long InsertRecord(long fkRun, int recordNumber, string recordName, string recordDescription)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Records (Fk_Run, RecordNumber,RecordName, RecordDescription)
            VALUES (@fkRun, @number,@name, @desc);
            SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@fkRun", fkRun);
            command.Parameters.AddWithValue("@number", recordNumber);
            command.Parameters.AddWithValue("@name", recordName);
            command.Parameters.AddWithValue("@desc", recordDescription);

            return (long)command.ExecuteScalar(); // returns the new Pk_Record
        }

        public static void LinkSignalToRun(long fkRun, long fkSignal)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT OR IGNORE INTO RunSignals (Fk_Run, Fk_Signal)
            VALUES (@fkRun, @fkSignal);";

            command.Parameters.AddWithValue("@fkRun", fkRun);
            command.Parameters.AddWithValue("@fkSignal", fkSignal);

            command.ExecuteNonQuery();
        }
        public static long GetRunPk(int runNumber)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT Pk_Run
            FROM Runs
            WHERE RunNumber = @runNumber;";

            command.Parameters.AddWithValue("@runNumber", runNumber);

            var result = command.ExecuteScalar();
            return result != null ? (long)result : -1; // Returns -1 if no run is found
        }

        public static int GetLastRunNumber()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT MAX(RunNumber)
            FROM Runs;";

            var result = command.ExecuteScalar();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0; // Returns 0 if no runs exist
        }

        public static int GetLastRecordOfRun(int runNumber)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT MAX(r.RecordNumber)
            FROM Records r
            JOIN Runs ru ON r.Fk_Run = ru.Pk_Run
            WHERE ru.RunNumber = @runNumber;";

            command.Parameters.AddWithValue("@runNumber", runNumber);

            var result = command.ExecuteScalar();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0; // Returns 0 if no records exist for the run
        }


        public static void UpdateRecordDataInSQLite(long pkRecord, string filePath)
        {
            // Read the binary file into a byte array
            byte[] recordData = File.ReadAllBytes(filePath);

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = new SqliteCommand(
                "UPDATE Records SET RecordData = @RecordData WHERE Pk_Record = @PkRecord",
                connection);

            // Add parameters
            command.Parameters.AddWithValue("@RecordData", recordData);
            command.Parameters.AddWithValue("@PkRecord", pkRecord);

            // Execute the query
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new Exception($"No record found with Pk_Record = {pkRecord}");
            }
        }

        public static Dictionary<int, List<Single>> ReadRecordDataFromSQLite(long pkRecord)
        {
            // Read binary data from SQLite
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT RecordData FROM Records WHERE Pk_Record = @PkRecord",
                connection);

            command.Parameters.AddWithValue("@PkRecord", pkRecord);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new Exception($"No record found with Pk_Record = {pkRecord}");
            }

            if (reader.IsDBNull(0))
            {
                throw new Exception($"No binary data found for Pk_Record = {pkRecord}");
            }

            // Read binary data into a byte array
            long dataLength = reader.GetBytes(0, 0, null, 0, 0);
            byte[] buffer = new byte[dataLength];
            reader.GetBytes(0, 0, buffer, 0, (int)dataLength);

            // Parse binary data into Dictionary<int, List<Single>>
            var result = new Dictionary<int, List<Single>>();
            using var ms = new MemoryStream(buffer);
            using var binaryReader = new BinaryReader(ms);

            try
            {
                while (ms.Position < ms.Length)
                {
                    int key = binaryReader.ReadInt32();
                    int count = binaryReader.ReadInt32();
                    var list = new List<Single>(count);

                    for (int i = 0; i < count; i++)
                    {
                        list.Add(binaryReader.ReadSingle());
                    }

                    if (!result.ContainsKey(key))
                    {
                        result[key] = new List<Single>();
                    }

                    result[key].AddRange(list);
                }
            }
            catch (EndOfStreamException)
            {
                throw new Exception($"Invalid binary data format for Pk_Record = {pkRecord}");
            }

            return result;
        }


        public static Dictionary<int, string> GetAllRunsIdx_Name()
        {
            var runs = new Dictionary<int, string>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT RunNumber, RunName FROM Runs ORDER BY RunNumber;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int runNumber = reader.GetInt32(0);
                string description = reader.IsDBNull(1) ? "" : reader.GetString(1);
                runs[runNumber] = description;
            }

            return runs;
        }


        public static Dictionary<int, string> GetAllRecordsOfRun(int runNumber)
        {
            var records = new Dictionary<int, string>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT r.RecordNumber, r.RecordDescription
            FROM Records r
            JOIN Runs ru ON r.Fk_Run = ru.Pk_Run
            WHERE ru.RunNumber = @runNumber
            ORDER BY r.RecordNumber;";

            command.Parameters.AddWithValue("@runNumber", runNumber);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int recordNumber = reader.GetInt32(0);
                string description = reader.IsDBNull(1) ? "" : reader.GetString(1);
                records[recordNumber] = description;
            }

            return records;
        }
    }
}
