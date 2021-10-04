using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Test.Api
{
    public class HttpClientFactoryBuilderOptions
    {
        public Action<IServiceCollection>? ConfigureServices { get; set; }
        public Dictionary<string, Action<HttpClient, IConfiguration>>? ConfigureNamedHttpClients { get; set; }
    }
}
