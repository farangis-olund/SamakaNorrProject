using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class SearchMessageEntity
{
    [Key]
    public int MessageId { get; set; }

    [Required]
    public string SenderId { get; set; } = null!;

    [Required]
    public string MessageContent { get; set; } = null!;

    [Required]
    public DateTime Timestamp { get; set; }

    public UserEntity User { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    // For search requests
    public int? SearchRequestId { get; set; }
    public SearchRequestEntity? SearchRequest { get; set; }


}