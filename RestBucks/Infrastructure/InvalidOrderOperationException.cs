namespace RestBucks.Infrastructure
{
  using System;

  public class InvalidOrderOperationException : Exception
  {
    public InvalidOrderOperationException() : this("no message provided")
    {
    }

    public InvalidOrderOperationException(string message)
      : base(message)
    {
    }
  }
}