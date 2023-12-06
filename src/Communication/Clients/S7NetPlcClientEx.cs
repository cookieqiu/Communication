using Communication.Exceptions.ClientEventArgs;
using Communication.Exceptions;
using Communication.Messages;
using Communication.Utils;

namespace Communication.Clients
{
    public class S7NetPlcClientEx : ClientBase
    {
        private S7.Net.Plc? _client;

        private int _writeDB;
        private string _address;
        private int _port;
        private S7.Net.CpuType _cpu;
        private short _rack;
        private short _slot;

        private int _heartBeatCount;
        private CancellationTokenSource? _abortToken;
        private readonly SemaphoreSlim _wSemaphore;
        private readonly SemaphoreSlim _rSemaphore;

        public override event EventHandler<OnExceptionEventArgs>? OnException;
        public override event EventHandler<OnConnectedEventArgs>? OnConnected;
        public override event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
        public override event EventHandler<OnWritedEventArgs>? OnWrited;
        public override event EventHandler<OnReadedEventArgs>? OnReaded;

        public S7NetPlcClientEx()
        {
            _address = string.Empty;
            _wSemaphore = new SemaphoreSlim(1);
            _rSemaphore = new SemaphoreSlim(1);
        }

        public override bool IsConnected => _client?.IsConnected ?? false;

        public void HandleException(Exception ex)
        {
            if (OnException is null) throw ex;
            OnException.Invoke(this, new OnExceptionEventArgs(ex));
        }

        public override bool Connect(string connectionString)
        {
            InitConnectionInfo(connectionString);
            return InternalConnect();
        }

        public override Task<bool> ConnectAsync(string connectionString, CancellationToken cancellationToken)
            => Task.Run(() => Connect(connectionString), cancellationToken);

        public override bool DisConnect()
            => InternalDisconnect();

        public override Task<bool> DisConnectAsync(CancellationToken cancellationToken)
            => Task.Run(InternalDisconnect, cancellationToken);

        public override bool Read(MessageBase message)
            => InternalRead(message);

        public override Task<bool> ReadAsync(MessageBase message, CancellationToken cancellationToken)
            => Task.Run(() => InternalRead(message), cancellationToken);

        public override bool Write(MessageBase message)
            => InternalWrite(message);

        public override Task<bool> WriteAsync(MessageBase message, CancellationToken cancellationToken)
            => Task.Run(() => InternalWrite(message), cancellationToken);

        protected override void OnDisposing()
        {
            _client?.Close();
            _abortToken?.Dispose();
            _wSemaphore?.Dispose();
            _wSemaphore?.Dispose();
        }

        private void InitConnectionInfo(string connectionString)
        {
            var parser = ConnectionStringHelper.Parse(connectionString);
            _writeDB = parser.GetInt("WriteDB");
            _address = parser.GetString("IP");
            _port = parser.GetInt("Port");
            _cpu = (S7.Net.CpuType)parser.GetInt("CpuType");
            _rack = parser.GetShort("Rack");
            _slot = parser.GetShort("Slot");
        }

        protected bool InternalConnect()
        {
            if (IsConnected)
            {
                return true;
            }
            if (string.IsNullOrEmpty(_address))
            {
                throw new InvalidOperationException("Not set ip.");
            }
            try
            {
                _client = new S7.Net.Plc(_cpu, _address, _port, _rack, _slot)
                {
                    WriteTimeout = (int)WriteTimeout.TotalMilliseconds,
                    ReadTimeout = (int)ReadTimeout.TotalMilliseconds
                };
                _client.Open();

                _abortToken = new();
                BeginCheckHeartbeat();

                OnConnected?.Invoke(this, new OnConnectedEventArgs());
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return false;
        }

        protected bool InternalDisconnect()
        {
            try
            {
                _abortToken!.Cancel();
                _client?.Close();
                OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs());
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return false;
        }

        protected bool InternalRead(MessageBase message)
        {
            _rSemaphore.Wait();
            if (!IsConnected)
            {
                InternalConnect();
            }
            try
            {
                var buffer = message.CreateBuffer();
                var readDB = buffer.ConvertTo<int>();
                var data = _client!.ReadBytes(S7.Net.DataType.DataBlock
                    , readDB, 0, buffer.Length);

                Array.Copy(data, 0, buffer, 0, buffer.Length);
                message.Deserialize(buffer);
                OnReaded?.Invoke(this, new(buffer));

                Heartbeat();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                _rSemaphore.Release();
            }
            return false;
        }

        protected bool InternalWrite(MessageBase message)
        {
            _wSemaphore.Wait();
            if (!IsConnected)
            {
                InternalConnect();
            }
            try
            {
                var buffer = message.Serialize();

                _client!.WriteBytes(S7.Net.DataType.DataBlock
                    , _writeDB, 0, buffer);
                OnWrited?.Invoke(this, new(buffer));

                Heartbeat();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                _wSemaphore.Release();
            }
            return false;
        }

        private void Heartbeat() => _heartBeatCount = 0;

        private void BeginCheckHeartbeat()
        {
            Task.Run(async () =>
            {
                while (!_abortToken!.IsCancellationRequested)
                {
                    if (_heartBeatCount > 3)
                    {
                        HandleException(new HeartbeatException());
                        break;
                    }
                    else if (_heartBeatCount > 0)
                    {
                        if (HeartbeatFunc is null || HeartbeatFunc.Invoke())
                        {
                            Heartbeat();
                        }
                    }
                    await Task.Delay(HeartBeatDelay);
                    _heartBeatCount++;
                }
            }, _abortToken!.Token);
        }
    }
}
