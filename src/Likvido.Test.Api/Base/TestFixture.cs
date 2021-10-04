using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Likvido.Test.Api
{
    public abstract class TestFixture<TStartup, TDesignContext, TRuntimeContext> : TestFixtureBase
        where TStartup : class
        where TDesignContext : TRuntimeContext
        where TRuntimeContext : DbContext
    {
        private HttpClientFactory<TStartup>? _clientBuilder { get; set; }
        protected HttpClientFactory<TStartup> HttpClientFactory
        {
            get
            {
                if (_clientBuilder == null)
                {
                    throw new InvalidOperationException($"Method \"{nameof(TestStarted)}\" should be called first");
                }

                return _clientBuilder;
            }

        }

        public virtual HttpClient Client => HttpClientFactory.HttpClient;

        public TRuntimeContext Context
        {
            get
            {
                var options = GetFixtureOptions();
                if (options?.DatabaseFixture?.Context == null)
                {
                    throw new InvalidOperationException("No context was configured for this fixture");
                }
                return options.DatabaseFixture.Context;
            }
        }

        protected override void OnTestStarted()
        {
            var configuration = GetFixtureOptions();
            _clientBuilder ??= HttpClientFactoryBuilder<TStartup>.Build(new HttpClientFactoryBuilderOptions
            {
                ConfigureServices = s =>
                {
                    ConfigureMocks(s);
                    configuration?.ConfigureServices?.Invoke(s);
                    if (configuration?.DatabaseFixture != null)
                    {
                        s.AddScoped(_ => configuration.DatabaseFixture.CreateRuntimeContext());
                    }
                },
                ConfigureNamedHttpClients = configuration?.ConfigureNamedHttpClients
            });

            GetFixtureOptions()?.DatabaseFixture?.CreateTestData();
        }

        protected override void OnTestEnded()
        {
            GetFixtureOptions()?.DatabaseFixture?.Cleanup();
            ResetMocks();
        }

        protected virtual FixtureOptions<TDesignContext, TRuntimeContext>? GetFixtureOptions()
        {
            return null;
        }

        protected override void OnDisposing()
        {
            base.OnDisposing();
            GetFixtureOptions()?.DatabaseFixture?.Dispose();
            CleanBuilder();
        }

        protected void CleanBuilder()
        {
            _clientBuilder?.Dispose();
            _clientBuilder = null;
        }

        private List<(Type ServiceType, Mock Mock)>? _mocks;

        private void ConfigureMocks(IServiceCollection services)
        {
            _mocks = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => typeof(Mock).IsAssignableFrom(x.PropertyType))
                .Select(x => (x.PropertyType.GetGenericArguments()[0], (Mock)x.GetValue(this)!))
                .ToList();

            _mocks.ForEach(x => services.AddSingleton(x.ServiceType, x.Mock.Object));
        }

        private void ResetMocks()
        {
            _mocks?.ForEach(x => x.Mock.Reset());
        }
    }
}
