namespace UniNotes.Services;

public static class LanguageService
{
    public const string CookieName = ".AspNetCore.Culture";

    public static string GetCurrentCulture(HttpContext context)
    {
        var culture = context.Request.Cookies[CookieName];
        if (!string.IsNullOrEmpty(culture))
        {
            var parts = culture.Split('|');
            if (parts.Length == 2 && parts[0].StartsWith("c="))
                return parts[0].Substring(2);
        }
        return "bg";
    }
}