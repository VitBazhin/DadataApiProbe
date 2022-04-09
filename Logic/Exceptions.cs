using System;
using System.Runtime.Serialization;

namespace DadataApiProbe.Logic
{
    [Serializable]
    internal class ParameterException : Exception
    {
        public ParameterException()
        {
        }

        public ParameterException(string message) : base(message)
        {
        }

        public ParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class ConfigException : Exception
    {
        public override string Message { get; }
        public ConfigException() { Message = "Configuration item not found"; }

    }

}