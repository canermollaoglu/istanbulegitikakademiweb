﻿using log4net.Core;
using System;

namespace NitelikliBilisim.Core.CrossCuttingConcerns.Logging.Log4Net
{
    [Serializable]
    public class SerializableLogEvent
    {
        private readonly LoggingEvent _loggingEvent;
        public SerializableLogEvent(LoggingEvent loggingEvent)
        {
            _loggingEvent = loggingEvent;
        }
        public object Message => _loggingEvent.MessageObject;
    }
}
