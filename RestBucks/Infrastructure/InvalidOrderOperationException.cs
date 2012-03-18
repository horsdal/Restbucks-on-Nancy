namespace RestBucks.Infrastructure
{
  using System;

  public class InvalidOrderOperationException : Exception
  {
    public InvalidOrderOperationException(string message)
      : base(message)
    {
    }
  }
}