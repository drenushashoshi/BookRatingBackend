namespace FBookRating.Models.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } // Auto-generate unique IDs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Set creation time
        public DateTime? UpdatedAt { get; set; } // Nullable for updates
    }

}
