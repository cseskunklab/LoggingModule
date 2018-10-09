using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdgeLoggingServerModule.Controllers.EdgeLoggingServerErrorEvent
{
    /*public class EdgeLoggingServerErrorEvent
    {
        public delegate void EdgeLoggingServerEventHandler(object o, EdgeLoggingServerEventArgs edgeEventArgs);

        public event
    }*/

    public class EdgeLoggingServerEventArgs : EventArgs
    {
        private Exception _loggingException;
        private string _loggingExceptionMsg;

        public EdgeLoggingServerEventArgs(Exception LoggingServerException, string LoggingExceptionMsg)
        {
            _loggingException = LoggingServerException;
            _loggingExceptionMsg = LoggingExceptionMsg;
        }

        public Exception EdgeLoggingServerException
        {
            get { return _loggingException; }
        }

        public string EdgeLoggingServerExceptionMessage
        {
            get { return _loggingExceptionMsg; }
        }
    }
}
