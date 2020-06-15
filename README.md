[![](https://img.shields.io/badge/.NET%20Core-3.1-brightgreen.svg?style=flat-square)](https://www.microsoft.com/net/download/core) 
[![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE) 

# ketchup -- 番茄酱
### ketchup 是一个基于dotnet core的微服务开发框架。 让概念变成实践，让实践变得简单。
### 网关：kong
### rpc：grpc支持远程调用，
### 注册中心：consul，
### 负载均衡算法：轮询、随机。
### 缓存：redis、memory，
### 消息中间件：rabbitmq
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
                            options.Listen(new IPEndPoint(IPAddress.Parse(config.Ip), config.Port),
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
            services.AddGrpc();
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
       "Port": "5003"//服务端口
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
更多文档请转移到 [WiKi](https://github.com/simple-gr/ketchup/wiki).
