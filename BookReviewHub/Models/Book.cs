using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookReviewHub.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public int PublishedYear { get; set; }
        [Required]
        public string Genre { get; set; }
        public ICollection<Review> Reviews { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
