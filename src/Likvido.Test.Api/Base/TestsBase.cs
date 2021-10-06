using System;

namespace Likvido.Test.Api
{
    public abstract class TestsBase<TFixture> : IDisposable
        where TFixture : TestFixtureBase
    {
        protected bool IsPersistentFixture { get; set; }
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected TFixture? _fixture;
#pragma warning restore CA1051 // Do not declare visible instance fields

        public TFixture Fixture
        {
            get
            {
                if (_fixture == null)
                {
                    throw new InvalidOperationException("Fixture must be configured before usage");
                }

                return _fixture;
            }
        }

        protected TestsBase()
        {
            IsPersistentFixture = false;
        }

        protected TestsBase(TFixture fixture)
        {
            _fixture = fixture;
            IsPersistentFixture = true;
            InitializeFixture();
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

            _fixture?.TestEnded();
            if (!IsPersistentFixture)
            {
                _fixture?.Dispose();
                _fixture = null;
            }
        }

        protected void InitializeFixture()
        {
            _fixture?.TestStarted();
        }
    }
}
