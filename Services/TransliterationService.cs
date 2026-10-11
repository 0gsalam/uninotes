using System.Text;

namespace UniNotes.Services;

/// <summary>
/// Двупосочна транслитерация между български (кирилица) и латиница.
/// Използва "Обтекаема система" за BG → Latin и обратна логика за Latin → BG.
/// </summary>
public static class TransliterationService
{
    // ============================================
    // BG → LATIN (char → string)
    // ============================================
    private static readonly Dictionary<char, string> BgToLat = new()
    {
        { 'а', "a" },  { 'б', "b" },  { 'в', "v" },  { 'г', "g" },
        { 'д', "d" },  { 'е', "e" },  { 'ж', "zh" }, { 'з', "z" },
        { 'и', "i" },  { 'й', "y" },  { 'к', "k" },  { 'л', "l" },
        { 'м', "m" },  { 'н', "n" },  { 'о', "o" },  { 'п', "p" },
        { 'р', "r" },  { 'с', "s" },  { 'т', "t" },  { 'у', "u" },
        { 'ф', "f" },  { 'х', "h" },  { 'ц', "ts" }, { 'ч', "ch" },
        { 'ш', "sh" }, { 'щ', "sht" }, { 'ъ', "a" },  { 'ь', "y" },
        { 'ю', "yu" }, { 'я', "ya" },
    };

    // ============================================
    // LATIN → BG (string → string!) — най-дългите първи
    // ============================================
    private static readonly (string Lat, string Bg)[] LatToBg = new[]
    {
        ("sht", "щ"), ("sh", "ш"), ("ch", "ч"), ("ts", "ц"),
        ("zh", "ж"),  ("yu", "ю"), ("ya", "я"), ("ks", "кс"),
        ("a", "а"),   ("b", "б"),  ("v", "в"),  ("g", "г"),
        ("d", "д"),   ("e", "е"),  ("z", "з"),  ("i", "и"),
        ("y", "й"),   ("k", "к"),  ("l", "л"),  ("m", "м"),
        ("n", "н"),   ("o", "о"),  ("p", "п"),  ("r", "р"),
        ("s", "с"),   ("t", "т"),  ("u", "у"),  ("f", "ф"),
        ("h", "х"),   ("c", "ц"),  ("w", "в"),  ("q", "к"),
    };

    /// <summary>
    /// Превръща кирилица в латиница. "Бележка" → "Belezhka"
    /// </summary>
    public static string ToLatin(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var sb = new StringBuilder(input.Length * 2);
        foreach (var ch in input)
        {
            var lower = char.ToLowerInvariant(ch);

            if (BgToLat.TryGetValue(lower, out var lat))
            {
                if (char.IsUpper(ch) && lat.Length > 0)
                {
                    sb.Append(char.ToUpperInvariant(lat[0]));
                    if (lat.Length > 1) sb.Append(lat.Substring(1));
                }
                else
                {
                    sb.Append(lat);
                }
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Превръща латиница в кирилица. "Belezhka" → "Бележка"
    /// </summary>
    public static string ToCyrillic(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var lower = input.ToLowerInvariant();
        var sb = new StringBuilder(lower.Length);
        int i = 0;

        while (i < lower.Length)
        {
            bool matched = false;

            foreach (var (lat, bg) in LatToBg)
            {
                if (i + lat.Length <= lower.Length &&
                    string.CompareOrdinal(lower, i, lat, 0, lat.Length) == 0)
                {
                    if (char.IsUpper(input[i]) && bg.Length > 0)
                        sb.Append(char.ToUpperInvariant(bg[0]));
                    else if (bg.Length > 0)
                        sb.Append(bg[0]);

                    if (bg.Length > 1) sb.Append(bg.Substring(1));

                    i += lat.Length;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                sb.Append(input[i]);
                i++;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Генерира всички варианти на търсения текст за максимално покритие.
    /// </summary>
    public static List<string> GetSearchVariants(string query)
    {
        var variants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(query))
            return new List<string>();

        query = query.Trim();

        variants.Add(query);

        var latin = ToLatin(query);
        if (!string.Equals(latin, query, StringComparison.OrdinalIgnoreCase))
            variants.Add(latin);

        var cyr = ToCyrillic(query);
        if (!string.Equals(cyr, query, StringComparison.OrdinalIgnoreCase))
            variants.Add(cyr);

        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var v in variants)
        {
            if (!string.IsNullOrWhiteSpace(v))
                result.Add(v.ToLowerInvariant());
        }

        return result.ToList();
    }
}