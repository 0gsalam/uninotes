using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UniNotes.ViewModels;

public class NoteFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int? CourseId { get; set; }

    public string? Tags { get; set; }

    public bool IsPinned { get; set; }

    public SelectList? Courses { get; set; }
}