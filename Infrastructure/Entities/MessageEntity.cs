using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class MessageEntity
{
    [Key]
    public int MessageId { get; set; }

    [Required]
    public string SenderId { get; set; } = null!;

	[Required]
	public string ReceiverId { get; set; } = null!;

	[Required]
    public int RideId { get; set; }

    [Required]
    public string MessageContent { get; set; } = null!;

    [Required]
    public DateTime Timestamp { get; set; }

    public UserEntity User { get; set; } = null!;
    public RideEntity Ride { get; set; } = null!;
    public bool IsRead { get; set; } = false;


}
