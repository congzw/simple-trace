namespace SimpleTrace.TraceClients
{
    public class TraceConfig
    {
        public TraceConfig()
        {
            TraceSaveProcessEnabled = true;
            TraceSendProcessEnabled = false;
            TraceEndPoint = "http://localhost:14268/api/traces";
            DefaultTracerId = "CT-Default";
            FlushIntervalSecond = 5;
            TraceArchiveFolder = "Trace";
        }

        public bool TraceSaveProcessEnabled { get; set; }
        public bool TraceSendProcessEnabled { get; set; }
        public string TraceEndPoint { get; set; }
        public string DefaultTracerId { get; set; }
        public int FlushIntervalSecond { get; set; }
        public string TraceArchiveFolder { get; set; }
        
        public static TraceConfig CreateDefaultForLinux()
        {
            var traceConfig = new TraceConfig
            {
                TraceSaveProcessEnabled = true,
                TraceSendProcessEnabled = false,
                FlushIntervalSecond = 10,
                DefaultTracerId = "CT-Linux"
        };
            return traceConfig;
        }

        public static TraceConfig CreateDefaultForWindows()
        {
            var traceConfig = new TraceConfig
            {
                TraceSaveProcessEnabled = true,
                TraceSendProcessEnabled = true,
                FlushIntervalSecond = 5,
                DefaultTracerId = "CT-Win"
            };
            return traceConfig;
        }
    }
}
