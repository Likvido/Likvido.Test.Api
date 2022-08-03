using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;

namespace Likvido.Test.Api.Sqlite
{
#pragma warning disable CA1812 // SqliteMigrationGenerator is an internal class that is apparently never instantiated.
    internal class SqliteMigrationGenerator : MigrationsSqlGenerator
#pragma warning restore CA1812 // SqliteMigrationGenerator is an internal class that is apparently never instantiated.
    {
        private readonly IDictionary<string, string> _replacements = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["newsequentialid()"] = "",
            ["getdate()"] = "CURRENT_TIMESTAMP",
            ["getutcdate()"] = "CURRENT_TIMESTAMP"
        };

        public SqliteMigrationGenerator(MigrationsSqlGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override void DefaultValue(object? defaultValue, string? defaultValueSql, string? columnType, MigrationCommandListBuilder builder)
        {
            if (defaultValueSql != null &&
                Dependencies.SqlGenerationHelper is SqliteSqlGenerationHelper &&
                _replacements.TryGetValue(defaultValueSql, out var replacement)) // I assume you only want to adapt specific values rather than whole type
            {
                if (string.IsNullOrEmpty(replacement))
                {
                    return;
                }
                builder
                    .Append(" DEFAULT (")
                    .Append(replacement)
                    .Append(")");
                return;
            }
            //fall back to default implementation
            base.DefaultValue(defaultValue, defaultValueSql, columnType, builder);
        }
    }
}
