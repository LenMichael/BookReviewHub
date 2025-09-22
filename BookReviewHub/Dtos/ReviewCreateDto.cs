using System.ComponentModel.DataAnnotations;

namespace BookReviewHub.Dtos
{
    public class ReviewCreateDto
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        public int BookId { get; set; }
    }
}
