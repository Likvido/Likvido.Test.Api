using System;
using System.Collections.Generic;
using System.Net.Http;
using Likvido.Test.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Test.Api
{
    public class FixtureOptions<TDesignContext, TRuntimeContext>
        where TDesignContext : TRuntimeContext
        where TRuntimeContext : DbContext
    {
        public DatabaseFixture<TDesignContext, TRuntimeContext>? DatabaseFixture { get; set; }
        public Action<IServiceCollection>? ConfigureServices { get; set; }
        public Dictionary<string, Action<HttpClient, IConfiguration>> ConfigureNamedHttpClients { get; set; } = new();
    }
}
