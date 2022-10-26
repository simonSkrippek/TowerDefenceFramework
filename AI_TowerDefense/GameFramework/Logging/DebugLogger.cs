using System;
using System.IO;
using Threading;
using AI_TowerDefense;

namespace GameFramework
{
    public static class DebugLogger
    {
        private static LogCache logCache = new LogCache(TowerDefense.DISPLAY_LOG_MESSAGES);

        public static void Log(double message, bool renderDirectly = false)
        {
            DebugLogger.Log(message.ToString(), renderDirectly);
        }

        public static void Log(float message, bool renderDirectly = false)
        {
            DebugLogger.Log(message.ToString(), renderDirectly);
        }

        public static void Log(int message, bool renderDirectly = false)
        {
            DebugLogger.Log(message.ToString(), renderDirectly);
        }

        public static void Log(string message, bool renderDirectly = false)
        {
            DebugLogger.logCache.Log(message);
            DebugLogger.WriteToFile(message);

            if (renderDirectly)
                DebugLogger.RenderCache();
        }

        private static void WriteToFile(string message)
        {
            if (TowerDefense.DISPLAY_LOG_MESSAGES)
            {
                try
                {
                    StreamWriter logFile = File.AppendText(LogCache.FILE_NAME);
                    logFile.WriteLine(message);
                    logFile.Close();
                }
                catch (Exception exception) { }
            }
        }

        public static void RenderCache()
        {
            if (TowerDefense.DISPLAY_LOG_MESSAGES)
            {
                Console.SetCursorPosition(68, 0);
                Console.Write("Logs:");

                for (int row = 0; row < 24; row++)
                {
                    Console.SetCursorPosition(68, row + 2);
                    Console.Write("                                                  ");
                    Console.SetCursorPosition(68, row + 2);
                    Console.Write(DebugLogger.logCache.GetLogMessage(row));
                }
            }
        }
    }
}
