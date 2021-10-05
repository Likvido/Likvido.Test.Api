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
        where TDesignContext : DbContext
        where TRuntimeContext : DbContext
    {
        protected TestFixture()
        {
            InitMocks();
        }

        private HttpClientFactory<TStartup>? _httpClientFactory;
        protected HttpClientFactory<TStartup> HttpClientFactory
        {
            get
            {
                if (_httpClientFactory == null)
                {
                    throw new InvalidOperationException($"Method \"{nameof(TestStarted)}\" should be called first");
                }

                return _httpClientFactory;
            }

        }

        public virtual HttpClient Client => HttpClientFactory.HttpClient;

        public TDesignContext Context
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
            ResetMocks();
            var fixtureOptions = GetFixtureOptions();
            _httpClientFactory ??= new HttpClientFactory<TStartup>(new HttpClientFactoryOptions
            {
                ConfigureServices = s =>
                {
                    ConfigureMocks(s);
                    fixtureOptions?.ConfigureServices?.Invoke(s);
                    if (fixtureOptions?.DatabaseFixture != null)
                    {
                        s.AddScoped(_ => fixtureOptions.DatabaseFixture.CreateRuntimeContext());
                    }
                },
                ConfigureNamedHttpClients = fixtureOptions?.ConfigureNamedHttpClients
            });

            GetFixtureOptions()?.DatabaseFixture?.CreateTestData();
        }

        protected override void OnTestEnded()
        {
            GetFixtureOptions()?.DatabaseFixture?.Cleanup();
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
            _httpClientFactory?.Dispose();
            _httpClientFactory = null;
        }

        private List<(Type ServiceType, Mock Mock)>? _mocks;

        private void InitMocks()
        {
            _mocks = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => typeof(Mock).IsAssignableFrom(x.PropertyType))
                .Select(x => (x.PropertyType.GetGenericArguments()[0], (Mock)x.GetValue(this)!))
                .ToList();
        }

        private void ConfigureMocks(IServiceCollection services)
        {
            _mocks?.ForEach(x => services.AddSingleton(x.ServiceType, x.Mock.Object));
        }

        /// <summary>
        /// Called on before starting each test. Resets all mocks to default state. 
        /// </summary>
        protected virtual void ResetMocks()
        {
            _mocks?.ForEach(x => x.Mock.Reset());
        }
    }
}
