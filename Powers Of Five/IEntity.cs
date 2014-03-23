using System;

namespace Ross.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface all entities must implement.
    /// </summary>
    public interface IEntity : IIdentifiable, ICompositeEntity
    {
        DateTime CreatedDate { get; }

        int CreatedBy { get; }
    }
}