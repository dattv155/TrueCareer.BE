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
    public class FavouriteNewsHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(FavouriteNewsHandler);

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
                List<FavouriteNews> FavouriteNews = JsonConvert.DeserializeObject<List<FavouriteNews>>(json);
                if (FavouriteNews != null && FavouriteNews.Count > 0)
                    await UOW.FavouriteNewsRepository.BulkMerge(FavouriteNews);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(FavouriteNewsHandler));
            }
        }

    }
}
