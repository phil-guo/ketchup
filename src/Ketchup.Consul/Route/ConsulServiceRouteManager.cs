using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Ketchup.Consul.ClientProvider;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.Utilities;
using Ketchup.Core.Address;
using Ketchup.Core.Route;
using Ketchup.Core.Route.Implementation;
using Ketchup.Core.Serialization;
using Microsoft.Extensions.Logging;

namespace Ketchup.Consul.Route
{
    /// <summary>
    ///     consul路由管理
    /// </summary>
    public class ConsulServiceRouteManager : ServiceRouteManagerBase, IDisposable
    {
        private readonly ISerializer<byte[]> _serializer;
        //private readonly IServiceHeartbeatManager _serviceHeartbeatManager;

        private readonly AppConfig _appConfig;
        private readonly IConsulClientProvider _consulClientProvider;
        private readonly ISerializer<string> _stringSerializer;
        private readonly ILogger<ConsulServiceRouteManager> _logger;
        private readonly IServiceRouteFactory _serviceRouteFactory;
        private ServiceRoute[] _routes;

        public ConsulServiceRouteManager(ISerializer<string> serializer,
            IConsulClientProvider consulClientProvider,
            IServiceRouteFactory serviceRouteFactory,
            AppConfig appConfig,
            ILogger<ConsulServiceRouteManager> logger,
            ISerializer<byte[]> serializer1)
            : base(serializer)
        {
            _appConfig = appConfig;
            _consulClientProvider = consulClientProvider;
            _serviceRouteFactory = serviceRouteFactory;
            _logger = logger;
            _serializer = serializer1;
            _stringSerializer = serializer;


            EnterRoutes().Wait();
        }

        public void Dispose()
        {
        }

        public override Task<IEnumerable<ServiceRoute>> GetRoutesAsync()
        {
            throw new NotImplementedException();
        }

        public override Task RemveAddressAsync(IEnumerable<AddressModel> Address)
        {
            throw new NotImplementedException();
        }

        public override Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task SetRoutesAsync(IEnumerable<ServiceRouteDescriptor> routes)
        {
            throw new NotImplementedException();
        }

        public async Task EnterRoutes()
        {
            if (_routes != null && _routes.Length > 0)
                return;
            Action<string[]> action = null;
            var client = await GetConsulClient();

            if ((await client.KV.Keys(_appConfig.Consul.ServicePath)).Response?.Length > 0)
            {
                var result = await client.GetChildrenAsync(_appConfig.Consul.ServicePath);
                var keys = await client.KV.Keys(_appConfig.Consul.ServicePath);
                var childrens = result;
                action?.Invoke(ConvertPaths(childrens).Result.Select(key => $"{_appConfig.Consul.ServicePath}{key}").ToArray());
                _routes = await GetRoutes(keys.Response);
            }
            else
            {
                if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
                    _logger.LogWarning($"无法获取路由信息，因为节点：{_appConfig.Consul.ServicePath}，不存在。");
                _routes = new ServiceRoute[0];
            }
        }

        private async Task<ServiceRoute[]> GetRoutes(IEnumerable<string> childrens)
        {
            childrens = childrens.ToArray();
            var routes = new List<ServiceRoute>(childrens.Count());

            foreach (var children in childrens)
            {
                if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                    _logger.LogDebug($"准备从节点：{children}中获取路由信息。");

                var route = await GetRoute(children);
                if (route != null)
                    routes.Add(route);
            }

            return routes.ToArray();
        }

        private async Task<ServiceRoute> GetRoute(string path)
        {
            ServiceRoute result = null;
            var client = await GetConsulClient();
            //var watcher = new NodeMonitorWatcher(GetConsulClient, _manager, path,
            //    async (oldData, newData) => await NodeChange(oldData, newData), tmpPath =>
            //    {
            //        var index = tmpPath.LastIndexOf("/");
            //        return _serviceHeartbeatManager.ExistsWhitelist(tmpPath.Substring(index + 1));
            //    });

            var queryResult = await client.KV.Keys(path);
            if (queryResult.Response != null)
            {
                var data = await client.GetDataAsync(path);
                if (data != null)
                {
                    //watcher.SetCurrentData(data);
                    result = await GetRoute(data);
                }
            }

            return result;
        }

        private async Task<ServiceRoute> GetRoute(byte[] data)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                _logger.LogDebug($"准备转换服务路由，配置内容：{Encoding.UTF8.GetString(data)}。");

            if (data == null)
                return null;

            var descriptor = _serializer.Deserialize<byte[], ServiceRouteDescriptor>(data);
            return (await _serviceRouteFactory.CreateServiceRoutesAsync(new[] { descriptor })).First();
        }

        private async ValueTask<ConsulClient> GetConsulClient()
        {
            var client = await _consulClientProvider.GetClient();
            return client;
        }

        /// <summary>
        ///     转化路径集合
        /// </summary>
        /// <param name="datas">信息数据集合</param>
        /// <returns>返回路径集合</returns>
        private async Task<string[]> ConvertPaths(string[] datas)
        {
            var paths = new List<string>();
            foreach (var data in datas)
            {
                var result = await GetRouteData(data);
                var serviceId = result?.ServiceDescriptor.Id;
                if (!string.IsNullOrEmpty(serviceId))
                    paths.Add(serviceId);
            }

            return paths.ToArray();
        }

        private async Task<ServiceRoute> GetRouteData(string data)
        {
            if (data == null)
                return null;

            var descriptor =
                _stringSerializer.Deserialize(data, typeof(ServiceRouteDescriptor)) as ServiceRouteDescriptor;
            return (await _serviceRouteFactory.CreateServiceRoutesAsync(new[] { descriptor })).First();
        }
    }
}