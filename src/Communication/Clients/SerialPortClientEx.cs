using Communication.Exceptions;
using Communication.Exceptions.ClientEventArgs;
using Communication.Messages;
using Communication.Utils;
using System.IO.Ports;

namespace Communication.Clients
{
    public class SerialPortClientEx : ClientBase
    {
        private SerialPort? _client;

        private string _portName;
        private int _baudRate;
        private Parity _parity;
        private int _dataBits;
        private StopBits _stopBits;

        private int _heartBeatCount;
        private CancellationTokenSource? _abortToken;
        private readonly SemaphoreSlim _wSemaphore;
        private readonly SemaphoreSlim _rSemaphore;

        public override event EventHandler<OnExceptionEventArgs>? OnException;
        public override event EventHandler<OnConnectedEventArgs>? OnConnected;
        public override event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
        public override event EventHandler<OnWritedEventArgs>? OnWrited;
        public override event EventHandler<OnReadedEventArgs>? OnReaded;

        public SerialPortClientEx()
        {
            _portName = string.Empty;
            _wSemaphore = new SemaphoreSlim(1);
            _rSemaphore = new SemaphoreSlim(1);
        }

        public override bool IsConnected => _client?.IsOpen ?? false;

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
            _portName = parser.GetString("PortName");
            _baudRate = parser.GetInt("BaudRate");
            _parity = (Parity)parser.GetInt("Parity");
            _dataBits = parser.GetInt("DataBits");
            _stopBits = (StopBits)parser.GetInt("StopBits");
        }

        protected bool InternalConnect()
        {
            if (IsConnected)
            {
                return true;
            }
            if (string.IsNullOrEmpty(_portName))
            {
                throw new InvalidOperationException("Not set port name.");
            }
            try
            {
                _client = new SerialPort
                {
                    PortName = _portName,
                    BaudRate = _baudRate,
                    Parity = _parity,
                    DataBits = _dataBits,
                    StopBits = _stopBits,
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

                _client!.Read(buffer, 0, buffer.Length);
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

                _client!.Write(buffer, 0, buffer.Length);
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
