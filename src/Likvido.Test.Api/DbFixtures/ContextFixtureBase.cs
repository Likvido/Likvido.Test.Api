using System;
using Microsoft.EntityFrameworkCore;

namespace Likvido.Test.Api
{
    public abstract class ContextFixtureBase<TDesignContext, TRuntimeContext> : IDisposable
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        private TDesignContext? _designContext;
        private bool _migrated;

        public TDesignContext DesignContext
        {
            get
            {
                if (_designContext == null)
                {
                    ResetDesignContext();
                }
                return _designContext!;
            }
        }

        public abstract TDesignContext GenerateDesignContext();
        public abstract TRuntimeContext GenerateRuntimeContext();

        public void Migrate(TDesignContext context)
        {
            if (_migrated)
            {
                return;
            }

            ExecuteMigration(context);
            _migrated = true;
        }

        private TDesignContext CreateDesignContext()
        {
            var designContext = GenerateDesignContext();
            Migrate(designContext);
            return designContext;
        }

        public TRuntimeContext CreateRuntimeContext()
        {
            return GenerateRuntimeContext();
        }

        public void CreateTestData()
        {
            GetTestDataHelper().CreateTestData();
        }

        public void Cleanup()
        {
            GetTestDataHelper().Cleanup();
            ResetDesignContext();
        }

        public abstract void ExecuteMigration(TDesignContext context);

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

            _designContext?.Dispose();
            _designContext = null;
        }

        protected abstract ITestDataHelper GetTestDataHelper();

        public virtual string Quote(string value)
        {
            return $"[{value}]";
        }

        private void ResetDesignContext()
        {
            _designContext?.Dispose();
            _designContext = CreateDesignContext();
        }
    }
}
