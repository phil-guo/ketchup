using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ketchup.RabbitMQ.Internal.Client.Implementation
{
    public class DefaultRabbitMqClientProvider : IRabbitMqClientProvider
    {
        public event EventHandler<ShutdownEventArgs> OnRabbitConnectionShutdown;
        private readonly IConnectionFactory _connectionFactory;
        IConnection _connection;
        bool _disposed;
        static object sync_root = new object();

        public DefaultRabbitMqClientProvider(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            lock (sync_root)
            {
                _connection = _connectionFactory
                    .CreateConnection();

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;


            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;


            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            OnRabbitConnectionShutdown(sender, reason);
            TryConnect();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
