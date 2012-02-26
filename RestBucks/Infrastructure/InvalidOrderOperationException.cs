using System;

namespace RestBucks.Infrastructure
{
    public class InvalidOrderOperationException : Exception
    {
        public InvalidOrderOperationException(string message)
            : base(message)
        {}
    }
}