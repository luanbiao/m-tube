using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class TratarTextos
{
    public static string SanitizeFileName(string fileName)
    {
        // Lista de caracteres inválidos adicionando também caracteres especiais comuns
        string invalidChars = new string(Path.GetInvalidFileNameChars()) + "!@#$%^&*()+=[]{}|;:'\",<>/?`~";

        // Remove caracteres inválidos do nome do arquivo e substitui por underscore
        string regexPattern = $"[{Regex.Escape(invalidChars)}]";
        string sanitizedFileName = Regex.Replace(fileName, regexPattern, "_");

        // Substitui espaços por underscores
        sanitizedFileName = sanitizedFileName.Replace(" ", "_");
        sanitizedFileName = sanitizedFileName.Replace("!", "");

        // Adicionalmente, substitui acentos por caracteres normais
        sanitizedFileName = RemoveDiacritics(sanitizedFileName);

        if (sanitizedFileName.Length > 15)
        {
            sanitizedFileName = sanitizedFileName.Substring(0, 15);

        }
        return sanitizedFileName;
    }

    public static string FormatFilePath(string path)
    {
        // Wrap path with double quotes
        string formattedPath = $"\"{path}\"";
        Debug.WriteLine($"Formatted path: {formattedPath}");
        return formattedPath;
    }

    public static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
