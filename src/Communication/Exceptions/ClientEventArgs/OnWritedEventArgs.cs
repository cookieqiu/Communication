namespace Communication.Exceptions.ClientEventArgs
{
    public class OnWritedEventArgs : EventArgs
    {
        public OnWritedEventArgs(in byte[] buffer)
        {
            Data = buffer;
        }
        public byte[] Data { get; }
    }
}