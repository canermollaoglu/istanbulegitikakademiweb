using System;
using System.Linq;

namespace NitelikliBilisim.Core.Services
{
    public  class StringHelper
    {
        private static char[] _validCharacters = { '\'', '\"', '!', '^', '+', '#', '$', '%', '&', '/', '{', '}', '(', ')', '[', ']', '=', '?', '*', '\\', '-', '_', '~', ',', ';', '´', '.', ':', '|', '<', '>', '@', '€', '¨', ' ' };
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
            sonuc = sonuc.Replace(" ", "_");
            sonuc = sonuc.Replace("  ", "_");
            sonuc = sonuc.Replace("   ", "_");
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
            sonuc = sonuc.Replace(".", "_");
            sonuc = sonuc.Replace("?", "_");
            sonuc = sonuc.Replace(";", "_");
            sonuc = sonuc.Replace("#", "_sharp");

            return sonuc;
        }
        public static string ClearHiddenCharacters(string text)
        {
            return new string(text.Where(x => char.IsLetter(x) || char.IsNumber(x) || _validCharacters.Contains(x)).ToArray());
        }
    }
}
