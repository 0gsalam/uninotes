using System.ComponentModel.DataAnnotations;

namespace UniNotes.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(7)]
    public string Color { get; set; } = "#7c5cff";

    public string UserId { get; set; } = string.Empty;

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}