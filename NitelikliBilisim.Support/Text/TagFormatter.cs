using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Support.Text
{
    public static class TagFormatter
    {
        public static string FormatForTag(this string input)
        {
            var text = input.ToLower().Trim();
            text = CharacterConverter(text);
            var splitted = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var result = "";
            for (int i = 0; i < splitted.Length; i++)
                result += i == splitted.Length - 1 ? splitted[i] : $"{splitted[i]}-";

            return result;
        }

        public static string CharacterConverter(string input)
        {
            return input
                .Replace("\"", "")
                .Replace("!", "")
                .Replace("'", "")
                .Replace("^", "")
                .Replace("#", "sharp")
                .Replace("+", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("&", "")
                .Replace("/", "")
                .Replace("{", "")
                .Replace("(", "")
                .Replace("[", "")
                .Replace(")", "")
                .Replace("]", "")
                .Replace("=", "")
                .Replace("}", "")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("\\", "")
                .Replace("_", "")
                .Replace("@", "")
                .Replace("€", "")
                .Replace("~", "")
                .Replace("¨", "")
                .Replace("´", "")
                .Replace(";", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace(".", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ç", "c")
                .Replace("ğ", "g");
        }
    }
}
