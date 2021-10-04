using System;

namespace Likvido.Test.Api
{
    public abstract class TestFixtureBase : IDisposable
    {
        public void TestStarted()
        {
            OnTestStarted();
        }

        public void TestEnded()
        {
            OnTestEnded();
        }

        protected virtual void OnTestEnded()
        {
        }

        protected virtual void OnTestStarted()
        {
        }


        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void OnDisposing()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    OnDisposing();
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
