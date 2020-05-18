# ketcup
### ketchup 是一个微服务框架，grpc提供服务远程调用，采用consul作为注册中心，采用哈希，轮询，随机作为负载均衡算法。使用redis作为分布式缓存，rabbitmq 作为消息中间件
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
更多说明请转移到 [WiKi](https://github.com/simple-gr/ketchup/wiki).

喜欢请给个start给予一点支持，qq群：592407137，期待大家一起学习研究
