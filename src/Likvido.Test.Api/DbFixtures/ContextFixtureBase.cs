using System;
using Microsoft.EntityFrameworkCore;

namespace Likvido.Test.Api
{
    public abstract class ContextFixtureBase<TDesignContext, TRuntimeContext> : IDisposable
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        private TDesignContext? _designContext;
        private TRuntimeContext? _runtimeContext;
        private bool _migrated;

        public TRuntimeContext RuntimeContext
        {
            get
            {
                if (_runtimeContext == null)
                {
                    ResetContext();
                }
                return _runtimeContext!;
            }
        }

        public TDesignContext DesignContext
        {
            get
            {
                if (_designContext == null)
                {
                    ResetContext();
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

        public (TDesignContext Design, TRuntimeContext Runtime) Create()
        {
            var designContext = GenerateDesignContext();
            Migrate(designContext);
            var runtimeContext = GenerateRuntimeContext();
            return (designContext, runtimeContext);
        }

        public void CreateTestData()
        {
            GetTestDataHelper().CreateTestData();
        }

        public void Cleanup()
        {
            GetTestDataHelper().Cleanup();
            ResetContext();
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
            _runtimeContext?.Dispose();
            _designContext = null;
            _runtimeContext = null;
        }

        protected abstract ITestDataHelper GetTestDataHelper();

        public virtual string Quote(string value)
        {
            return $"[{value}]";
        }

        private void ResetContext()
        {
            _designContext?.Dispose();
            _runtimeContext?.Dispose();
            (_designContext, _runtimeContext) = Create();
        }
    }
}
