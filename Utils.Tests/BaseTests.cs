using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Utils.Models;
using Z.EntityFramework.Extensions;

namespace Utils.Tests
{
    public class BaseTests
    {
        private readonly DbConnection _connection;
        public DataContext DataContext;
        public void Init()
        {
            LicenseManager.AddLicense("2456;100-FPT", "3f0586d1-0216-5005-8b7a-9080b0bedb5e");
            string licenseErrorMessage;
            if (!LicenseManager.ValidateLicense(out licenseErrorMessage))
            {
                throw new Exception(licenseErrorMessage);
            }


            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            DataContext = new DataContext(options);
            DataContext.Database.EnsureDeleted();
            DataContext.Database.EnsureCreated();
            EntityFrameworkManager.ContextFactory = DbContext => new DataContext(options);
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
