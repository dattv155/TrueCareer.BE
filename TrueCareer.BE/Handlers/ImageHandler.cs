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
    public class ImageHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(ImageHandler);

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
                List<Image> Images = JsonConvert.DeserializeObject<List<Image>>(json);
                if (Images != null && Images.Count > 0)
                    await UOW.ImageRepository.BulkMerge(Images);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(ImageHandler));
            }
        }

    }
}
