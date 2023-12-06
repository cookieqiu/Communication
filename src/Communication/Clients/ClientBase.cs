using Communication.Exceptions.ClientEventArgs;
using Communication.Messages;

namespace Communication.Clients
{
    public abstract class ClientBase : IClient
    {
        public abstract bool IsConnected { get; }
        public TimeSpan WriteTimeout { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(1);
        public Func<bool>? HeartbeatFunc { get; set; }
        public TimeSpan HeartBeatDelay { get; set; } = TimeSpan.FromMinutes(1);

        public abstract event EventHandler<OnExceptionEventArgs>? OnException;
        public abstract event EventHandler<OnConnectedEventArgs>? OnConnected;
        public abstract event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
        public abstract event EventHandler<OnWritedEventArgs>? OnWrited;
        public abstract event EventHandler<OnReadedEventArgs>? OnReaded;

        public abstract bool Connect(string connectionString);
        public abstract Task<bool> ConnectAsync(string connectionString, CancellationToken cancellationToken);
        public abstract bool DisConnect();
        public abstract Task<bool> DisConnectAsync(CancellationToken cancellationToken);
        public abstract bool Read(MessageBase message);
        public abstract Task<bool> ReadAsync(MessageBase message, CancellationToken cancellationToken);
        public abstract bool Write(MessageBase message);
        public abstract Task<bool> WriteAsync(MessageBase message, CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDisposing();
                }
                disposedValue = true;
            }
        }

        protected virtual void OnDisposing() { }

        ~ClientBase()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue;
    }
}
