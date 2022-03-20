using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.BE.Models;
using TrueCareer.Helpers;
using TrueCareer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using TrueSight.Common;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture;

namespace TrueCareer.Handlers
{
    public class MyRabbitMQHostedService : RabbitMQHostedService
    {
        private IConfiguration Configuration;
        internal List<IHandler> Handlers = new List<IHandler>();
        public MyRabbitMQHostedService(IPooledObjectPolicy<IModel> objectPolicy, IConfiguration Configuration): base(objectPolicy, StaticParams.ModuleName)
        {
            this.Configuration = Configuration;
            List<Type> handlerTypes = typeof(MyRabbitMQHostedService).Assembly.GetTypes()
                .Where(x => typeof(Handler).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                .ToList();
            foreach (Type type in handlerTypes)
            {
                Handler handler = (Handler)Activator.CreateInstance(type);
                Handlers.Add(handler);
            }

            foreach (IHandler handler in Handlers)
            {
                handler.QueueBind(_channel, StaticParams.ModuleName, "exchange");
            }
            _channel.BasicQos(0, 1, false);
        }

        protected override async Task HandleMessage(string routingKey, string content)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"), sqlOptions =>
            {
                sqlOptions.AddTempTableSupport();
            });
            DataContext context = new DataContext(optionsBuilder.Options);
            IUOW UOW = new UOW(context, Configuration);
            ICurrentContext CurrentContext = new CurrentContext
            {
                UserId = 0,
                UserName = "SYSTEM",
            };
            List<string> path = routingKey.Split(".").ToList();
            if (path.Count < 1)
                throw new Exception();
            foreach (IHandler handler in Handlers)
            {
                if (path.Any(p => p == handler.Name))
                {
                    handler.RabbitManager = RabbitManager;
                    await handler.Handle(UOW, routingKey, content);
                }
            }
        }
    }

    public class MyRabbitModelPooledObjectPolicy : RabbitModelPooledObjectPolicy
    {
        public MyRabbitModelPooledObjectPolicy(IConfiguration Configuration)
            : base(Configuration["RabbitConfig:Hostname"],
                 int.Parse(Configuration["RabbitConfig:Port"]),
                 Configuration["RabbitConfig:Username"],
                 Configuration["RabbitConfig:Password"],
                 Configuration["RabbitConfig:VirtualHost"])
        {

        }
    }
}
