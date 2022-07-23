using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrueCareer.BE.Models;
using System;
using System.Data.Common;
using Z.EntityFramework.Extensions;
using TrueCareer.Repositories;
using TrueCareer.Common;
using Microsoft.Extensions.Configuration;
using TrueCareer.Helpers;

namespace TrueCareer.Tests
{
    public class BaseTests
    {
        private readonly DbConnection _connection;
        public DataContext DataContext;
        public IUOW UOW;
        public ICurrentContext CurrentContext;
        public ILogging Logging;
        public void Init()
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables().Build();
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            DataContext = new DataContext(options);
            DataContext.Database.EnsureDeleted();
            DataContext.Database.EnsureCreated();
            EntityFrameworkManager.ContextFactory = DbContext => new DataContext(options);

            CurrentContext = new CurrentContext
            {
                UserId = 2,
                UserName = "ecchi123"
            };
            UOW = new UOW(DataContext, Configuration);
            Logging = new Logging(CurrentContext);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }
}
