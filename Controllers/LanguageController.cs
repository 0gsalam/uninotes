using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace UniNotes.Controllers;

public class LanguageController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl)
    {
        if (string.IsNullOrEmpty(culture) || (culture != "bg" && culture != "en"))
            culture = "bg";

        var requestCulture = new RequestCulture(culture, culture);

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(requestCulture),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

        return LocalRedirect(returnUrl ?? "/");
    }
}