namespace Communication.Exceptions.ClientEventArgs
{
    public class OnReadedEventArgs : EventArgs
    {
        public OnReadedEventArgs(in byte[] buffer)
        {
            Data = buffer;
        }
        public byte[] Data { get; }
    }
}