using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Entities;
using TrueCareer.Models;
using TrueCareer.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Handlers
{
    public class MentorMenteeConnectionHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(MentorMenteeConnectionHandler);

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
                List<MentorMenteeConnection> MentorMenteeConnections = JsonConvert.DeserializeObject<List<MentorMenteeConnection>>(json);
                if (MentorMenteeConnections != null && MentorMenteeConnections.Count > 0)
                    await UOW.MentorMenteeConnectionRepository.BulkMerge(MentorMenteeConnections);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(MentorMenteeConnectionHandler));
            }
        }

    }
}
