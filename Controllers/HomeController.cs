using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniNotes.Data;

namespace UniNotes.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User)!;

            ViewBag.TotalNotes = await _db.Notes.CountAsync(n => n.UserId == userId);
            ViewBag.TotalCourses = await _db.Courses.CountAsync(c => c.UserId == userId);
            ViewBag.PinnedNotes = await _db.Notes.CountAsync(n => n.UserId == userId && n.IsPinned);
            ViewBag.RecentNotes = await _db.Notes
                .Include(n => n.Course)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.UpdatedAt)
                .Take(5)
                .ToListAsync();
        }
        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy() => View();
}