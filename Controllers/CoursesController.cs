using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniNotes.Data;
using UniNotes.Models;

namespace UniNotes.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public CoursesController(ApplicationDbContext db, UserManager<IdentityUser> um)
    {
        _db = db;
        _userManager = um;
    }

    private string UserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index()
    {
        var courses = await _db.Courses
            .Include(c => c.Notes)
            .Where(c => c.UserId == UserId)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(courses);
    }

    public IActionResult Create() => View(new Course());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid) return View(course);

        course.UserId = UserId;
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var course = await _db.Courses
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id) return BadRequest();
        if (!ModelState.IsValid) return View(course);

        var existing = await _db.Courses
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId);
        if (existing == null) return NotFound();

        existing.Name = course.Name;
        existing.Code = course.Code;
        existing.Color = course.Color;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Notes)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _db.Courses
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId);
        if (course != null)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}