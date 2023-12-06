namespace Communication.Exceptions.ClientEventArgs
{
    public class OnExceptionEventArgs : EventArgs
    {
        public Exception Data { get; }
        public OnExceptionEventArgs(Exception exception)
        {
            Data = exception;
        }
    }
}
