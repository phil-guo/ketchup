using System;
using RabbitMQ.Client;

namespace Ketchup.RabbitMQ.Internal.Client
{
    public interface IRabbitMqClientProvider : IDisposable
    {
        bool IsConnected { get; }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建通道
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();

        event EventHandler<ShutdownEventArgs> OnRabbitConnectionShutdown;
    }
}
