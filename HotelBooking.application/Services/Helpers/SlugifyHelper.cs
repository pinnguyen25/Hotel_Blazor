using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class SlugifyHelper
{
    public static string Slugify(this string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // 1. Chuyển Unicode sang dạng chuẩn
        string normalized = text.Normalize(NormalizationForm.FormD);

        // 2. Loại bỏ dấu
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        // 3. Chuyển chữ Đ / đ thành D / d
        sb.Replace('Đ', 'D').Replace('đ', 'd');

        // 4. Chuyển thành chữ thường
        string slug = sb.ToString().ToLowerInvariant();

        // 5. Thay các ký tự không phải chữ cái, số bằng dấu '-'
        slug = Regex.Replace(slug, @"[^a-z0-9]+", "-").Trim('-');

        return slug;
    }
}
