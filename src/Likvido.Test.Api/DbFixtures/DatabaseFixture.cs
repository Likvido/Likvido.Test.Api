using System;
using Microsoft.EntityFrameworkCore;

namespace Likvido.Test.Api
{
    public abstract class DatabaseFixture<TDesignContext, TRuntimeContext> : IDisposable
        where TDesignContext : TRuntimeContext
        where TRuntimeContext : DbContext
    {
        public abstract ContextFixtureBase<TDesignContext, TRuntimeContext> ContextFixture { get; }

        public TRuntimeContext Context => ContextFixture.RuntimeContext;

        public abstract DatabaseFixtureType DbType { get; }

        public TRuntimeContext CreateRuntimeContext()
        {
            return ContextFixture.Create().Runtime;
        }

        public void CreateTestData()
        {
            ContextFixture.CreateTestData();
        }

        public void Cleanup()
        {
            ContextFixture.Cleanup();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ContextFixture?.Dispose();
        }
    }
}
