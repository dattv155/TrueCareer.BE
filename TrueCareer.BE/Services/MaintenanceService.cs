using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.BE.Models;
using TrueCareer.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Enums;

namespace TrueCareer.Services
{
    public interface IMaintenanceService : IServiceScoped
    {
        Task CleanHangfire();
        Task CleanExpiredConnection();
    }
    public class MaintenanceService : IMaintenanceService
    {
        private DataContext DataContext;
        public MaintenanceService(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task CleanHangfire()
        {
            var commandText = @"
                TRUNCATE TABLE [HangFire].[AggregatedCounter]
                TRUNCATE TABLE[HangFire].[Counter]
                TRUNCATE TABLE[HangFire].[JobParameter]
                TRUNCATE TABLE[HangFire].[JobQueue]
                TRUNCATE TABLE[HangFire].[List]
                TRUNCATE TABLE[HangFire].[State]
                DELETE FROM[HangFire].[Job]
                DBCC CHECKIDENT('[HangFire].[Job]', reseed, 0)
                UPDATE[HangFire].[Hash] SET Value = 1 WHERE Field = 'LastJobId'";
            var result = await DataContext.Database.ExecuteSqlRawAsync(commandText);
        }
        
        public async Task CleanExpiredConnection()
        {
            try
            {
                DateTime now = StaticParams.DateTimeNow;
                if (now.Minute == 0 && now.Second == 0)
                {
                    long endActiveTimeId = now.Hour - 6;
                    List<MentorMenteeConnectionDAO> MentorMenteeConnectionDAOs = await DataContext
                        .MentorMenteeConnection.Where(x => x.ActiveTimeId == endActiveTimeId).ToListAsync();
                    
                    IEnumerable<long> ConversationIds = MentorMenteeConnectionDAOs.Select(x => x.ConversationId).Distinct().ToList().Cast<long>();
                    List<ConversationDAO> ConversationDAOs = await DataContext.Conversation.Where(x => ConversationIds.Contains(x.Id)).ToListAsync();
                    
                    foreach (ConversationDAO conversation in ConversationDAOs)
                    {
                        conversation.StatusId = StatusEnum.INACTIVE.Id;
                    }
                    await DataContext.Conversation.BulkMergeAsync(ConversationDAOs);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
