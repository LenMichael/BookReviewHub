using System.ComponentModel.DataAnnotations;

namespace BookReviewHub.Models
{
    public class BookCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public int PublishedYear { get; set; }
        [Required]
        public string Genre { get; set; }
    }
}
