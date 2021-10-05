using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Likvido.Test.Api.Sqlite
{
    public abstract class SqliteContextFixture<TDesignContext, TRuntimeContext> : ContextFixtureBase<TDesignContext, TRuntimeContext>
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        private readonly SqliteConnection _connection;

        protected SqliteContextFixture()
        {
            _connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString());
            _connection.Open();
        }

        public override TRuntimeContext GenerateRuntimeContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TRuntimeContext>();
            var options = optionsBuilder
                .UseSqlite(_connection)
                .Options;
            return (TRuntimeContext)Activator.CreateInstance(typeof(TRuntimeContext), options)!;
        }

        public override TDesignContext GenerateDesignContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDesignContext>();
            var options = optionsBuilder
                .UseSqlite(_connection)
                .ReplaceService<IMigrationsSqlGenerator, SqliteMigrationGenerator>()
                .Options;
            return (TDesignContext)Activator.CreateInstance(typeof(TDesignContext), options)!;
        }

        public override void ExecuteMigration(TDesignContext context)
        {
            context.Database.EnsureCreated();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
