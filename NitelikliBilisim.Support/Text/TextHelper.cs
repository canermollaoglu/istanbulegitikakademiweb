using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Support.Text
{
    public static class TextHelper
    {
        public static string RandomPasswordGenerator(int size = 6)
        {
            return Guid.NewGuid().ToString()
                .Replace("-", "")
                .Substring(0, size).ToUpper();
            //var random = Guid.NewGuid().ToString()
            //    .Replace("-", "")
            //    .Substring(0, size);
            //var last = random.Substring(size - 3, 3).ToUpper();
            //return $"{random.Substring(0, 3)}{last}";
        }
        public static string ConcatForUserName(string part1, string part2)
        {
            var splitted = part1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return $"{splitted[0]}{part2.ToLower()}";
        }
    }
}
