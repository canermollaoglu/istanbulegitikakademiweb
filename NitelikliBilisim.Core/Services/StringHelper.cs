using System;
using System.Linq;

namespace NitelikliBilisim.Core.Services
{
    public  class StringHelper
    {
        private static readonly char[] ValidCharacters = { '\'', '\"', '!', '^', '+', '#', '$', '%', '&', '/', '{', '}', '(', ')', '[', ']', '=', '?', '*', '\\', '-', '_', '~', ',', ';', '´', '.', ':', '|', '<', '>', '@', '€', '¨', ' ' };
        public static string GenerateUniqueCode()
        {
            string base64String = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            base64String = System.Text.RegularExpressions.Regex.Replace(base64String, "[/+=]", "");

            return base64String.ToLower(new System.Globalization.CultureInfo("en-US", false));
        }
        public static string UrlFormatConverter(string name)
        {
            string sonuc = name.ToLower();
            sonuc = ClearHiddenCharacters(sonuc);
            sonuc = sonuc.Replace("'", "");
            sonuc = sonuc.Replace(" ", "-");
            sonuc = sonuc.Replace("  ", "-");
            sonuc = sonuc.Replace("   ", "-");
            sonuc = sonuc.Replace("<", "");
            sonuc = sonuc.Replace(">", "");
            sonuc = sonuc.Replace("&", "");
            sonuc = sonuc.Replace("[", "");
            sonuc = sonuc.Replace("!", "");
            sonuc = sonuc.Replace("]", "");
            sonuc = sonuc.Replace("ı", "i");
            sonuc = sonuc.Replace("ö", "o");
            sonuc = sonuc.Replace("ü", "u");
            sonuc = sonuc.Replace("ş", "s");
            sonuc = sonuc.Replace("ç", "c");
            sonuc = sonuc.Replace("ğ", "g");
            sonuc = sonuc.Replace("İ", "I");
            sonuc = sonuc.Replace("Ö", "O");
            sonuc = sonuc.Replace("Ü", "U");
            sonuc = sonuc.Replace("Ş", "S");
            sonuc = sonuc.Replace("Ç", "C");
            sonuc = sonuc.Replace("Ğ", "G");
            sonuc = sonuc.Replace("|", "");
            sonuc = sonuc.Replace(".", "-");
            sonuc = sonuc.Replace("?", "-");
            sonuc = sonuc.Replace(";", "-");
            sonuc = sonuc.Replace("#", "-sharp");

            return sonuc;
        }
        public static string ClearHiddenCharacters(string text)
        {
            return new string(text.Where(x => char.IsLetter(x) || char.IsNumber(x) || ValidCharacters.Contains(x)).ToArray());
        }
        public static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            var words = text.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                var item = words[i];

                var capitalized = item[0].ToString().ToUpper();

                if (item.Length > 1)
                    capitalized += item.Substring(1, item.Length - 1);

                words[i] = capitalized;
            }

            return string.Join(' ', words);
        }
    }
}
