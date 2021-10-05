using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Test.Api
{
    public class FixtureOptions<TContext> : FixtureOptions<TContext, TContext>
        where TContext : DbContext
    {
    }

    public class FixtureOptions<TDesignContext, TRuntimeContext>
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        public DatabaseFixture<TDesignContext, TRuntimeContext>? DatabaseFixture { get; set; }
        public Action<IServiceCollection>? ConfigureServices { get; set; }
        public Action<IServiceCollection, Func<TRuntimeContext>>? ConfigureContext { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, Action<HttpClient, IConfiguration>> ConfigureNamedHttpClients { get; set; } = new();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
