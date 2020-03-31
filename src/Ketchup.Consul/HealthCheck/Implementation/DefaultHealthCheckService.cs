using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ketchup.Core.Address;

namespace Ketchup.Consul.HealthCheck.Implementation
{
    public class DefaultHealthCheckService : IHealthCheckService, IDisposable
    {
        public readonly ConcurrentDictionary<ValueTuple<string, int>, MonitorEntry> _dictionary = new ConcurrentDictionary<ValueTuple<string, int>, MonitorEntry>();

        private readonly int _timeout = 30000;
        private readonly Timer _timer;

        public DefaultHealthCheckService()
        {
            var timeSpan = TimeSpan.FromSeconds(60);

            _timer = new Timer(async item =>
                {
                    await HealthCheck(_dictionary.ToArray().Select(i => i.Value), _timeout);
                }, null, timeSpan, timeSpan);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Monitor(IpAddressModel address)
        {
            _dictionary.GetOrAdd(new ValueTuple<string, int>(address.Ip, address.Port), k => new MonitorEntry()
            {
                EndPoint = address.CreateEndPoint(),
                IsHealth = true
            });
        }

        public async ValueTask<bool> IsHealth(IpAddressModel address)
        {
            bool isHealth;

            if (_dictionary.TryGetValue(new ValueTuple<string, int>(address.Ip, address.Port), out var entry))
            {
                isHealth = await HealthCheck(address, _timeout, entry);
            }
            else
            {
                isHealth = await HealthCheck(address, _timeout, new MonitorEntry()
                {
                    EndPoint = address.CreateEndPoint(),
                    IsHealth = true
                });
            }
            return isHealth;
        }

        private static async Task<bool> HealthCheck(IpAddressModel address, int timeout, MonitorEntry entry)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = timeout
            })
            {
                try
                {
                    await socket.ConnectAsync(address.CreateEndPoint());
                    entry.UnhealthyTimes = 0;
                    entry.IsHealth = true;
                }
                catch
                {
                    entry.UnhealthyTimes++;
                    entry.IsHealth = false;
                }

                return entry.IsHealth;
            }
        }

        private async Task HealthCheck(IEnumerable<MonitorEntry> entrys, int timeout)
        {
            foreach (var entry in entrys)
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendTimeout = timeout
                })
                {
                    try
                    {
                        await socket.ConnectAsync(entry.EndPoint);
                        entry.UnhealthyTimes = 0;
                        entry.IsHealth = true;
                    }
                    catch
                    {
                        entry.UnhealthyTimes++;
                        entry.IsHealth = false;
                    }
                }
        }
    }
}