using System;
using System.IO;

namespace Threading
{
    public class LogCache
    {
        public static readonly string FILE_NAME = "logs.txt";
        private string[] messageBuffer;

        public LogCache(bool createLogFile)
        {
            this.messageBuffer = new string[24];

            if(createLogFile)
                this.CreateLogFile();
        }

        private void CreateLogFile()
        {
            try { File.WriteAllText(LogCache.FILE_NAME, ""); }
            catch (Exception exception) { }
        }

        public void Log(string message)
        {
            message = (message.Length > 50) ? message.Substring(0, 50) : message;

            Array.Copy(this.messageBuffer, 0, this.messageBuffer, 1, this.messageBuffer.Length - 1);
            this.messageBuffer[0] = message;
        }

        public string GetLogMessage(int bufferIndex)
        {
            if (bufferIndex < this.messageBuffer.Length)
                return this.messageBuffer[bufferIndex];

            return null;
        }
    }
}
