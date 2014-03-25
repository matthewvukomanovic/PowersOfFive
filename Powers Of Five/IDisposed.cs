using System;

namespace Powers_Of_Five.Core
{
    public interface IDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}