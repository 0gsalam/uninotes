using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniNotes.Data;
using UniNotes.Models;
using UniNotes.ViewModels;

namespace UniNotes.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public NotesController(ApplicationDbContext db, UserManager<IdentityUser> um)
    {
        _db = db;
        _userManager = um;
    }

    private string UserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index(string? search, int? courseId, bool? pinned)
{
    var query = _db.Notes
        .Include(n => n.Course)
        .Include(n => n.Tags)
        .Where(n => n.UserId == UserId);

    if (courseId.HasValue)
        query = query.Where(n => n.CourseId == courseId);

    if (pinned == true)
        query = query.Where(n => n.IsPinned);

    var notes = await query
        .OrderByDescending(n => n.IsPinned)
        .ThenByDescending(n => n.UpdatedAt)
        .ToListAsync();

    // Филтриране в паметта за пълна поддръжка на кирилица + транслитерация
    if (!string.IsNullOrWhiteSpace(search))
    {
        var variants = Services.TransliterationService
            .GetSearchVariants(search)
            .Select(v => v.ToLowerInvariant())
            .ToList();

        notes = notes.Where(n =>
        {
            var title = (n.Title ?? "").ToLowerInvariant();
            var content = (n.Content ?? "").ToLowerInvariant();
            var tagNames = n.Tags.Select(t => (t.Name ?? "").ToLowerInvariant());

            var titleLatin = Services.TransliterationService.ToLatin(title).ToLowerInvariant();
            var contentLatin = Services.TransliterationService.ToLatin(content).ToLowerInvariant();

            return variants.Any(v =>
                title.Contains(v) || titleLatin.Contains(v) ||
                content.Contains(v) || contentLatin.Contains(v) ||
                tagNames.Any(tg => tg.Contains(v))
            );
        }).ToList();
    }

    ViewBag.Courses = await _db.Courses
        .Where(c => c.UserId == UserId)
        .OrderBy(c => c.Name)
        .ToListAsync();

    ViewBag.Search = search;
    ViewBag.CourseId = courseId;
    ViewBag.Pinned = pinned;

    return View(notes);
}

    public async Task<IActionResult> Details(int id)
    {
        var note = await _db.Notes
            .Include(n => n.Course)
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note == null) return NotFound();
        return View(note);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new NoteFormViewModel
        {
            Courses = await GetCoursesSelectList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Courses = await GetCoursesSelectList();
            return View(vm);
        }

        var note = new Note
        {
            Title = vm.Title,
            Content = vm.Content,
            CourseId = vm.CourseId,
            UserId = UserId,
            IsPinned = vm.IsPinned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        note.Tags = await ResolveTags(vm.Tags);

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var note = await _db.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note == null) return NotFound();

        var vm = new NoteFormViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CourseId = note.CourseId,
            IsPinned = note.IsPinned,
            Tags = string.Join(", ", note.Tags.Select(t => t.Name)),
            Courses = await GetCoursesSelectList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoteFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.Courses = await GetCoursesSelectList();
            return View(vm);
        }

        var note = await _db.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note == null) return NotFound();

        note.Title = vm.Title;
        note.Content = vm.Content;
        note.CourseId = vm.CourseId;
        note.IsPinned = vm.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;
        note.Tags = await ResolveTags(vm.Tags);

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var note = await _db.Notes
            .Include(n => n.Course)
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note == null) return NotFound();
        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note != null)
        {
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePin(int id)
    {
        var note = await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (note == null) return NotFound();

        note.IsPinned = !note.IsPinned;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<SelectList> GetCoursesSelectList()
    {
        var courses = await _db.Courses
            .Where(c => c.UserId == UserId)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new SelectList(courses, "Id", "Name");
    }

    private async Task<List<Tag>> ResolveTags(string? tagsInput)
    {
        if (string.IsNullOrWhiteSpace(tagsInput)) return new List<Tag>();

        var names = tagsInput
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existing = await _db.Tags.Where(t => names.Contains(t.Name)).ToListAsync();
        var result = new List<Tag>(existing);

        foreach (var name in names)
        {
            if (!existing.Any(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                var tag = new Tag { Name = name };
                _db.Tags.Add(tag);
                result.Add(tag);
            }
        }

        return result;
    }
}