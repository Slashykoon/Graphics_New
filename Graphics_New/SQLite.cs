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
                    RecordDescription TEXT,
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

        public static long InsertRun(int runNumber, string description)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Runs (RunNumber, RunDescription)
            VALUES (@number, @desc);
            SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@number", runNumber);
            command.Parameters.AddWithValue("@desc", description);

            return (long)command.ExecuteScalar(); // returns the new Pk_Run
        }

        public static long InsertRecord(long fkRun, int recordNumber, string recordDescription)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Records (Fk_Run, RecordNumber, RecordDescription)
            VALUES (@fkRun, @number, @desc);
            SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@fkRun", fkRun);
            command.Parameters.AddWithValue("@number", recordNumber);
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

    }
}
