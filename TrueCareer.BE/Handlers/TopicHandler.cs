using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Entities;
using TrueCareer.BE.Models;
using TrueCareer.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Handlers
{
    public class TopicHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(TopicHandler);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(object obj, string routingKey, string content)
        {   
            IUOW UOW = (IUOW) obj;
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<Topic> Topics = JsonConvert.DeserializeObject<List<Topic>>(json);
                if (Topics != null && Topics.Count > 0)
                    await UOW.TopicRepository.BulkMerge(Topics);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(TopicHandler));
            }
        }

    }
}
