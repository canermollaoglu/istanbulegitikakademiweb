using System;

namespace NitelikliBilisim.Support.Text
{
    public static class TextHelper
    {
        public static string RandomPasswordGenerator(int size = 6)
        {
            return Guid.NewGuid().ToString()
                .Replace("-", "")
                .Substring(0, size).ToUpper();
        }
        public static string ConcatForUserName(string part1, string part2)
        {
            var splitted = part1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userName = CharacterConverter(splitted[0] + part2.ToLower());
            return userName;
        }
        public static string CharacterConverter(string name)
        {
            string sonuc = name.ToLower();
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
    }
}
