using System;

namespace Ross.Infrastructure.Core
{
    public interface IDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}