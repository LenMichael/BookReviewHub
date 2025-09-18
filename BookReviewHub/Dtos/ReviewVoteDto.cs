using System.ComponentModel.DataAnnotations;

namespace BookReviewHub.Dtos
{
    public class ReviewVoteDto
    {
        [Required]
        public bool IsUpvote { get; set; }
    }
}
