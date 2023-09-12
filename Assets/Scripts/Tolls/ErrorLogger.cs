using System.IO;
using UnityEngine;

namespace Tolls
{
    public class ErrorLogger : MonoBehaviour
    {
        private string logFilePath;

        void Start()
        {
            // Set the path for the log file
            logFilePath = Path.Combine(Application.dataPath, "error_log.txt");

            // Redirect Unity's error and exception log messages to our custom log method
            Application.logMessageReceived += LogErrorToFile;
        }

        void OnDestroy()
        {
            // Make sure to remove the log message handler when the script is destroyed
            Application.logMessageReceived -= LogErrorToFile;
        }

        void LogErrorToFile(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                // Create or open the log file and append the error message and stack trace
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[Type: {type}]");
                    writer.WriteLine($"[Time: {System.DateTime.Now}]");
                    writer.WriteLine($"[Message]: {logString}");
                    writer.WriteLine($"[Stack Trace]:\n{stackTrace}\n");
                }
            }
        }
    }
}