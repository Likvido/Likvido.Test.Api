using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Test.Api
{
    public abstract class ConfigurableTestFixture<TStartup, TDesignContext, TRuntimeContext>
        : TestFixture<TStartup, TDesignContext, TRuntimeContext>
        where TStartup : class
        where TDesignContext : TRuntimeContext
        where TRuntimeContext : DbContext
    {
        private FixtureOptions<TDesignContext, TRuntimeContext>? _fixtureOptions;

        protected ConfigurableTestFixture()
        {
        }

        protected ConfigurableTestFixture(FixtureOptions<TDesignContext, TRuntimeContext> fixtureOptions)
        {
            SetOptions(fixtureOptions);
        }

        protected ConfigurableTestFixture(DatabaseFixture<TDesignContext, TRuntimeContext> databaseFixture)
        {
            SetDbFixture(databaseFixture);
        }

        protected ConfigurableTestFixture(Action<IServiceCollection> configureServices)
        {
            SetConfigureServices(configureServices);
        }

        public void SetOptions(FixtureOptions<TDesignContext, TRuntimeContext> fixtureOptions)
        {
            _fixtureOptions = fixtureOptions;
            CleanBuilder();
        }

        public void SetConfigureServices(Action<IServiceCollection> configureServices)
        {
            var fixtureOptions = _fixtureOptions ?? new FixtureOptions<TDesignContext, TRuntimeContext>();
            fixtureOptions.ConfigureServices = configureServices;
            SetOptions(fixtureOptions);
        }

        public void SetDbFixture(DatabaseFixture<TDesignContext, TRuntimeContext> databaseFixture)
        {
            var fixtureOptions = _fixtureOptions ?? new FixtureOptions<TDesignContext, TRuntimeContext>();
            fixtureOptions.DatabaseFixture = databaseFixture;
            SetOptions(fixtureOptions);
        }

        protected override FixtureOptions<TDesignContext, TRuntimeContext>? GetFixtureOptions()
        {
            return _fixtureOptions;
        }
    }
}
