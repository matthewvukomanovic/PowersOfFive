using System;
using System.Runtime.Serialization;
using Ross.Infrastructure.Interfaces;

namespace Ross.Infrastructure.Core
{
    [DataContract]
    [Serializable]
    public abstract class CompositeEntity : ICompositeEntity
    {
        [DataMember]
        public DateTime LastUpdated { get; set; }

        [DataMember]
        public int LastUpdatedBy { get; set; }

        [DataMember]
        public int RowVersion { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        public static bool IsActiveEntity(CompositeEntity entity)
        {
            if (entity == null)
                return false;
            return entity.IsActive;
        }

        public virtual T MemberCloneEntity<T>() where T : CompositeEntity
        {
            return this.MemberwiseClone() as T;
        }
    }
}