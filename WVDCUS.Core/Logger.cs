using System;
using System.Diagnostics;

namespace WVDCUS.Core
{
    public sealed class Logger
    {
        const string EventLogSourceName = "WVDCUS";

        static Logger()
        {
            try
            {
                s_eventLog = new EventLog();
                if (!EventLog.SourceExists(EventLogSourceName))
                    EventLog.CreateEventSource(EventLogSourceName, "");
                s_eventLog.Source = EventLogSourceName;
                s_eventLog.Log = "";
            }
            catch (Exception) // Catch if not successful registration - CreateEventSource 
            {
                s_eventLog = null;
            }
        }

        public static void WriteException(Exception e)
        {
            var msg = e.ToString();

            System.Console.WriteLine(msg);
            Trace.WriteLine(msg);
            s_eventLog?.WriteEntry(msg, EventLogEntryType.Error);
        }

        public static void WriteInfo(string format, params object[] arg)
        {
            var msg = String.Format(format, arg);

            System.Console.WriteLine(msg);
            Trace.WriteLine(msg);
            s_eventLog?.WriteEntry(msg, EventLogEntryType.Information);
        }

        private static EventLog s_eventLog = null;
    }
}
