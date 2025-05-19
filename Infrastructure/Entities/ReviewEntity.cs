using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ReviewEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RideId { get; set; }

    [Required]
    public string ReviewerId { get; set; } = null!;

    [Required]
    public string ReviewedUserId { get; set; } = null!;

    [Required]
    public string ReviewContent { get; set; } = null!;

    [Required]
    public int Rating { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    public UserEntity User { get; set; } = null!;
}