using System;

namespace XComponent.Functions.Core.Exceptions
{
    public class FunctionsFactoryException: Exception
    {
        public FunctionsFactoryException(string message):
            base(message)
        { }

        public FunctionsFactoryException(string message, Exception exception) :
            base(message, exception)
        { }
    }
}
