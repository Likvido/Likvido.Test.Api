using Microsoft.EntityFrameworkCore;

namespace Likvido.Test.Api.Sqlite
{
    public class SqliteDatabaseFixture<TDesignContext, TRuntimeContext> : DatabaseFixture<TDesignContext, TRuntimeContext>
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        private readonly ContextFixtureBase<TDesignContext, TRuntimeContext> _contextFixture;

        public SqliteDatabaseFixture(
            SqliteContextFixture<TDesignContext, TRuntimeContext> contextFixture)
        {
            _contextFixture = contextFixture;
        }

        public override ContextFixtureBase<TDesignContext, TRuntimeContext> ContextFixture => _contextFixture;

        public override DatabaseFixtureType DbType => DatabaseFixtureType.Sqlite;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contextFixture?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
