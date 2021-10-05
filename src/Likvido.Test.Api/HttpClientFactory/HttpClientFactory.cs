using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Likvido.Test.Api
{
    public class HttpClientFactory<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly HttpClientFactoryOptions _options;
        private readonly WebApplicationFactory<TStartup> _factory;
        private readonly Dictionary<string, HttpClient> _namedHttpClients = new();

        private IConfiguration? _configuration;
        private HttpClient? _httpClient;

        public HttpClientFactory(HttpClientFactoryOptions options)
        {
            _options = options;
            _factory = CreateWebAppFactory();
        }

        public IConfiguration Configuration => _configuration ??= _factory.Services.GetRequiredService<IConfiguration>();

        public HttpClient HttpClient => _httpClient ??= _factory.CreateClient();

        public HttpClient GetNamedHttpClient(string name)
        {
            if (_namedHttpClients.TryGetValue(name, out var namedHttpClient))
            {
                return namedHttpClient;
            }

            if (_options.ConfigureNamedHttpClients != null &&
                _options.ConfigureNamedHttpClients.TryGetValue(name, out var httpClientConfig))
            {
                var httpClient = _factory.CreateClient();
                httpClientConfig.Invoke(httpClient, Configuration);
                _namedHttpClients[name] = httpClient;
                return httpClient;
            }
            else
            {
                throw new InvalidOperationException($"HttpClient {name} is not configured.");
            }
        }

        private WebApplicationFactory<TStartup> CreateWebAppFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

#pragma warning disable CA2000 // Dispose objects before losing scope
            return new WebApplicationFactory<TStartup>()
#pragma warning restore CA2000 // Dispose objects before losing scope
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, conf) =>
                    {
                        conf.AddJsonFile(configPath);
                    });

                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders(); // Remove loggers
                    });

                    builder.ConfigureTestServices((s) =>
                    {
                        _options.ConfigureServices?.Invoke(s);
                    });
                });
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

            _factory?.Dispose();
            _httpClient?.Dispose();

            var httpClients = _namedHttpClients.Values.ToList();
            _namedHttpClients.Clear();
            httpClients.ForEach(x => x.Dispose());
        }
    }
}
