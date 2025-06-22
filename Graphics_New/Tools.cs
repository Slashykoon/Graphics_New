using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_New
{
    public static class Tools
    {
        public static void LogToFile(string message)
        {
            try
            {
                // Get the directory where the executable is located
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Combine the executable directory with the log file name
                string filePath = Path.Combine(exeDirectory, "LogFile.txt");

                // Open the file and append the message
                using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' to append, 'false' to overwrite
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during logging
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}
