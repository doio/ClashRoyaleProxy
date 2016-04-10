using System;
using System.IO;
using System.Text;

namespace ClashRoyaleProxy
{
    class Logger
    {
        public static bool FileLogging = false; // TODO: Configfile!

        /// <summary>
        /// Logs passed text
        /// </summary>
        public static void Log(string text, LogType type, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = (type == LogType.EXCEPTION || type == LogType.WARNING) ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write("[" + type + "] ");
            Console.ResetColor();
            Console.WriteLine(text);

            if (FileLogging)
                LogToFile(text, type);
        }

        /// <summary>
        /// Logs passed text to a file
        /// </summary>
        public static void LogToFile(string text, LogType type)
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string path = Environment.CurrentDirectory + @"\\logs\\log_" + System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy") + "." + "log";
            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Append));
            sw.WriteLine("[" + System.DateTime.UtcNow.ToLocalTime().ToString("hh-mm-ss") + "-" + type + "] " + text);
            sw.Close();
        }
    }
}
