using System.ComponentModel.DataAnnotations;

namespace UniNotes.Models;

public class Note
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int? CourseId { get; set; }
    public Course? Course { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;

    public bool IsPinned { get; set; }
}