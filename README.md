[![](https://img.shields.io/badge/.NET%20Core-3.1-brightgreen.svg?style=flat-square)](https://www.microsoft.com/net/download/core) 
[![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/simple-gr/ketchup/blob/master/LICENSE) 

# ketchup -- 番茄酱
### 文档： [WiKi](https://github.com/simple-gr/ketchup/wiki).
### ketchup 是一个基于dotnet core的微服务框架。
### 网关：兼容 kong
### rpc：grpc支持远程调用，
### 注册中心：consul，
### 负载均衡算法：轮询、随机、加权随机算法。
### 缓存：redis、memory，
### 消息中间件：rabbitmq
### 附一个 RBAC 的服务设计实现 地址： [zero](https://github.com/simple-gr/ketchup.zero).
![image](https://github.com/simple-gr/ketchup/blob/master/images/design.jpg)

## 如何运行起来
## Program中
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());                    
                    config.AddJsonFile("config/server.json", true, true); 
                }).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options =>
                        {
                            var config = AppConfig.ServerOptions;
                            //使用http2协议
                            options.Listen(new IPEndPoint(IPAddress.Any, config.Port),
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                        })
                        .UseStartup<Startup>();
                });
        }
## Startup中
###  配置config
          public Startup(IConfiguration configuration)
          {
            AppConfig.Configuration = (IConfigurationRoot)configuration;
          }
### 添加grpc服务
          public void ConfigureServices(IServiceCollection services)
          {
            // Add things to the service collection.
             services.AddGrpc(grpc => grpc.Interceptors.Add<HystrixCommandIntercept>());
          }
### 添加autofa 注册服务
          public void ConfigureContainer(ContainerBuilder builder)
          {
            // Add things to the Autofac ContainerBuilder.
            builder.AddCoreService().RegisterModules();
          }
### 配置中间件
          public void Configure(IApplicationBuilder app)
          {
            // Set up the application for development.
            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();
            app.UseRouting();
            app.UseKetchupServer();
          }
## 服务端server.json配置
    "Server": {
       "Name": "sample",//服务名称
       "Ip": "127.0.0.1",//服务ip
       "Port": "5003",//服务端口
       "EnableHttp": true, //是否开启http
       "Security": {
          "Whitelist": "*", //白名单
          "BlackList": "" //黑名单
       }
     }
    "Consul": {
      "ConnectionString": "127.0.0.1:8500",//consul 的链接地址
      "IsHealthCheck": true//是否进行健康检查
    }
## 客户端client.json配置
     "Consul": {
         "ConnectionString": "127.0.0.1:8500",
         "IsHealthCheck": true
     }

