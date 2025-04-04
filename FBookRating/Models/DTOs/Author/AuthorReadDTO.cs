namespace FBookRating.Models.DTOs.Author
{
    public class AuthorReadDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
