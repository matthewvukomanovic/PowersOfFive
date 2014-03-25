using System;

namespace Powers_Of_Five.Core
{
    /// <summary>
    /// Implements abstract Disposable object that provides base implementation plus helpers to check for dispose
    /// </summary>
    public abstract class Disposable : IDisposed
    {
        private object locker = new object();

        ~Disposable()
        {
            if (!IsDisposed)
                Dispose();
        }

        bool _disposing;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (locker)
            {
                if (!IsDisposed && !_disposing)
                {
                    _disposing = true;

                    OnDispose();
                    GC.SuppressFinalize(this);

                    _disposing = false;
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Called when disposing the resource.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Ensures the object is not disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Thrown if it is disposed</exception>
        protected void EnsureNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        private bool _isDisposed;
        public bool IsDisposed
        {
            get
            {
                lock (locker)
                {
                    return _isDisposed;
                }
            }
        }
    }
}