﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Consul;

namespace CoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            String ip = Configuration["ip"];//部署到不同服务器的时候不能写成127.0.0.1或者0.0.0.0，因为这是让服务消费者调用的地址
            int port = int.Parse(Configuration["port"]);//获取服务端口
            var client = new ConsulClient(ConfigurationOverview); //回调获取
            string serverId = "ServerNameFirst" + Guid.NewGuid();

            var result = client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serverId,//服务编号保证不重复
                Name = "ProductService",//服务的名称
                Address = ip,//服务ip地址
                Port = port,//服务端口
                Check = new AgentServiceCheck //健康检查
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后反注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                    HTTP = $"http://{ip}:{port}/api/Health",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)//服务的注册时间
                }
            });

            lifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("注销方法");
                client.Agent.ServiceDeregister(serverId).Wait();//服务停止时取消注册
            });
        }


        private static void ConfigurationOverview(ConsulClientConfiguration obj)
        {
            //consul的地址
            obj.Address = new Uri("http://127.0.0.1:8500");
            //数据中心命名
            obj.Datacenter = "dc1";
        }
    }
}

