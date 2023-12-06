using Communication.Messages;

namespace Communication.Clients
{
    public interface IClient : IDisposable
    {
        bool Connect(string connectionString);
        Task<bool> ConnectAsync(string connectionString, CancellationToken cancellationToken);
        bool DisConnect();
        Task<bool> DisConnectAsync(CancellationToken cancellationToken);
        bool Write(MessageBase message);
        Task<bool> WriteAsync(MessageBase message, CancellationToken cancellationToken);
        bool Read(MessageBase message);
        Task<bool> ReadAsync(MessageBase message, CancellationToken cancellationToken);
    }
}
