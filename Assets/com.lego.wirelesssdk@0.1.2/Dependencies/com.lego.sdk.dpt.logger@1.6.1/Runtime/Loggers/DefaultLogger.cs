using System;

namespace LEGO.Logger.Loggers
{
    internal sealed class DefaultLogger : AbstractLogger
    {
        public DefaultLogger(Type type) : base(type) { }

        public override void LogMessage(object message, LogLevel logLevel, int messageLengthLimit=2048)
        {
            if (!IsLogLevelEnabled(logLevel)) 
                return;
            
            var logMessage = (message != null) ? message.ToString() : "(null)";
           
            if (messageLengthLimit <= 0)
            {   // unlimited log message length
                foreach (var appender in Appenders)
                    appender.Print(new LogMessage(this, logMessage, logLevel, DateTime.Now));
            }
            else
            {
               while (logMessage.Length > 0)
               {
                   var substringLength = Math.Min(logMessage.Length, messageLengthLimit);
                   var limitedMessage = logMessage.Substring(0, substringLength);
                   logMessage = logMessage.Remove(0, substringLength);

                   foreach (var appender in Appenders)
                       appender.Print(new LogMessage(this, limitedMessage, logLevel, DateTime.Now));
                }
            }
        }

        public override void LogMessage(object message, Exception t, LogLevel logLevel)
        {
            if (!IsLogLevelEnabled(logLevel)) 
                return;
            
            var logTxt = (message != null) ? message.ToString() : "(null)";
            var logMessage = new LogMessage(this, logTxt, logLevel, DateTime.Now, t);
            
            foreach (var appender in Appenders)
                appender.Print(logMessage);
        }                   


    }
}