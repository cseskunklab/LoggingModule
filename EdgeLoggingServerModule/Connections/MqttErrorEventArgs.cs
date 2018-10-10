using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdgeLoggingServerModule.Connections
{
    public class MqttErrorEventArgs : EventArgs
    {
        public MqttErrorEventArgs(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; internal set; }
    }
}
