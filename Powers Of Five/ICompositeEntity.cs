using System;

namespace Ross.Infrastructure.Interfaces
{
    public interface ICompositeEntity
    {
        DateTime LastUpdated { get; }

        int LastUpdatedBy { get; }

        int RowVersion { get; set; }

        bool IsActive { get; set; }
    }
}