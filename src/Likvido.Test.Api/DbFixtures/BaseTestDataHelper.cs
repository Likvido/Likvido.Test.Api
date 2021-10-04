using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Likvido.Test.Api
{
    public class BaseTestDataHelper<TContext> : ITestDataHelper
        where TContext : DbContext
    {
        private IDictionary<string, string[]>? _beforeCleanupCommands;
        private IDictionary<string, string[]>? _specialCleanupCommands;
        protected bool _testDataCreated;
        protected readonly Func<TContext> _context;
        protected readonly Func<string, string> _q;

        public BaseTestDataHelper(Func<TContext> context, Func<string, string> quote)
        {
            _context = context;
            _q = quote;
        }

        protected TContext Context => _context();

        public virtual void Cleanup()
        {
            var beforeCleanupCommands = GetBeforeCleanupCommands();
            var specialCleanupCommands = GetSpecialCleanupCommands();

            foreach (var tableName in GetCleanupTables())
            {
                if (beforeCleanupCommands.ContainsKey(tableName))
                {
                    foreach (var command in beforeCleanupCommands[tableName])
                    {
                        Context.Database.ExecuteSqlRaw(command);
                    }
                }

                if (_testDataCreated && specialCleanupCommands.ContainsKey(tableName))
                {
                    foreach (var command in specialCleanupCommands[tableName])
                    {
                        Context.Database.ExecuteSqlRaw(command);
                    }
                }
                else
                {
                    Context.Database.ExecuteSqlRaw($@"DELETE FROM {_q(tableName)}");
                }
            }
        }

        public virtual string[] GetCleanupTables()
        {
            return Array.Empty<string>();
        }

        public void CreateTestData()
        {
            if (_testDataCreated)
            {
                return;
            }

            Cleanup();

            GenerateTestDataAsync().Wait();

            _testDataCreated = true;
        }

        public virtual Task GenerateTestDataAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes foreign keys if required
        /// </summary>
        public virtual IDictionary<string, string[]> GenerateBeforeCleanupCommands()
        {
            return new Dictionary<string, string[]>();
        }

        /// <summary>
        /// If some test data need to be kept
        /// </summary>
        public virtual IDictionary<string, string[]> GenerateSpecialCleanupCommands()
        {
            return new Dictionary<string, string[]>();
        }

        private IDictionary<string, string[]> GetBeforeCleanupCommands()
        {
            return _beforeCleanupCommands ??= GenerateBeforeCleanupCommands();
        }

        private IDictionary<string, string[]> GetSpecialCleanupCommands()
        {
            return _specialCleanupCommands ??= GenerateSpecialCleanupCommands();
        }
    }
}
