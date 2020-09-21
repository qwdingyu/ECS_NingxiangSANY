using HHECS.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHECS.Model.Common
{
    public class Logger
    {
        #region 事件

        public static event Delegates.LogWriteEventHandle LogWrite;

        public static void Log(string content, LogLevel logLevel, Exception exception = null)
        {
            LogWrite?.Invoke(null, LogEventArgs.GetLogEventArgs(content, logLevel, exception));
        }

        public static void Log(LogTitle logTitle, string content, LogLevel logLevel, Exception exception = null)
        {
            LogWrite?.Invoke(null, LogEventArgs.GetLogEventArgs(logTitle, content, logLevel, exception));
        }
        #endregion
    }
}
