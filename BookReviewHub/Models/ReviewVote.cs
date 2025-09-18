using System.ComponentModel.DataAnnotations;

namespace BookReviewHub.Models
{
    public class ReviewVote
    {
        public int Id { get; set; }
        [Required]
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        [Required]
        public string UserId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
