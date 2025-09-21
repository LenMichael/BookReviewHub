using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviewHub.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public ICollection<ReviewVote> ReviewVotes { get; set; }

    }
}