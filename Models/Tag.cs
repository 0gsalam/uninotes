using System.ComponentModel.DataAnnotations;

namespace UniNotes.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}