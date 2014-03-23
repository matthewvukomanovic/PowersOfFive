using System;
using System.Runtime.Serialization;
using Ross.Infrastructure.Interfaces;

namespace Ross.Infrastructure.Core
{
    [DataContract]
    [Serializable]
    public abstract class Entity : CompositeEntity, IEntity
    {
        protected Entity()
        {

        }

        protected Entity(int id)
        {
            Id = id;
        }

        protected Entity(int id, int rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }

        /// <summary>
        /// The ID of this entity.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="ReferenceEntity"/>
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="ReferenceEntity"/>.</param>
        /// <returns><code>true</code> if the specified <see cref="System.Object"/> is equal to the current <see cref="ReferenceEntity"/>;
        /// otherwise <code>false</code>.</returns>
        /// <remarks>
        /// Equality is expanded in <see cref="ReferenceEntity"/> classes so two <see cref="ReferenceEntity"/> objects with the
        /// same <see cref="Entity.Id"/> are considered equal.
        /// </remarks>
        public override bool Equals(object obj)
        {
            var referenceEntity = obj as Entity;
            if (referenceEntity == null)
            {
                return false;
            }

            return (referenceEntity.Id == Id && referenceEntity.RowVersion == RowVersion);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ReferenceEntity"/>.</returns>
        /// <remarks>
        /// Must be overridden since we are changing the functionality in <see cref="Equals"/>.
        /// </remarks>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">The first <see cref="Entity"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="Entity"/> to compare, or null.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Entity a, Entity b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">The first <see cref="Entity"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="Entity"/> to compare, or null.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Entity a, Entity b)
        {
            return !Equals(a, b);
        }

        public bool HasId
        {
            get
            {
                return Id > 0;
            }
        }
    }
}