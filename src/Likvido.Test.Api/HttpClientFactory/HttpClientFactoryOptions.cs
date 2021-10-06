using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Test.Api
{
    public class HttpClientFactoryOptions
    {
        public Action<IServiceCollection>? ConfigureServices { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, Action<IServiceProvider, HttpClient>>? ConfigureNamedHttpClients { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
