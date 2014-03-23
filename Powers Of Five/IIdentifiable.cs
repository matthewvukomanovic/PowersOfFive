namespace Ross.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for items that can be identified by an id.
    /// </summary>
    public interface IIdentifiable<T>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        T Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an id.
        /// The entity won't have an id until it is persisted as the database is using auto-numbers.
        /// </summary>
        bool HasId { get; }
    }

    /// <summary>
    /// Interface for items that can be identified by an id.
    /// </summary>
    public interface IIdentifiable : IIdentifiable<int>
    {
    }
}
