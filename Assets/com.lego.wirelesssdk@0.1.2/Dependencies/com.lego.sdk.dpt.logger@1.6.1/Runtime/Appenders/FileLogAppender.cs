using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace LEGO.Logger.Appenders
{
    /// <summary>
    /// File IO log appender. Appends all logs into a file on disk, within <see cref="UnityEngine.Application.persistenDataPath"/>.
    /// A new file is created for each session, with one backup being maintained.
    /// The backup will be appended with "-previous.log"
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FileLogAppender : ILogAppender
    {
        private StreamWriter outputStream;

        public string LogFileName { get; private set; }
        public string LogFileNamePrevious { get; private set; }
        
        public FileLogAppender(string filenamePrefix)
        {
            LogFileName = Path.Combine(Application.persistentDataPath, filenamePrefix + ".log");
            LogFileNamePrevious = Path.Combine(Application.persistentDataPath, filenamePrefix + "-previous.log");

            SwitchLogFiles();
        }

        public LogLevel LevelFilter { get; set; }

        public void Print(ILogMessage message)
        {
            if (message.Level < LevelFilter)
                return;

            var msg = string.Format("[{0}]\t{1:d/M/yy HH:mm:ss.fff}\t{2}", message.ClassName, DateTime.Now,
                message.Text);

            if (message.Exception != null)
                outputStream?.WriteLine(message.Exception + "\n" + message.Exception.StackTrace);
            else
                outputStream?.WriteLine(message.Level + ": " + msg);

            outputStream?.Flush();
        }

        public void CloseLogFile()
        {
            outputStream?.Flush();
            outputStream?.Close();
            outputStream = null;
        }

        public void ReOpenLogFile()
        {
            CloseLogFile();
            
            try
            {
                outputStream = new StreamWriter(LogFileName, true);
            }
            catch (IOException e)
            {
                if((uint)e.HResult != 0x80070020 /*-2147024864*/ ) // sharing violation - ie already open
                {
                    throw; // not the exception we're looking for...
                }
            }
        }

        public void SwitchLogFiles()
        {
            CloseLogFile();
            if (File.Exists(LogFileName))
                File.Copy(LogFileName, LogFileNamePrevious, true);
            
            outputStream = new StreamWriter(LogFileName, false );
        }
    }
}
