using NitelikliBilisim.App.Models;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Lexicographer
{
    public static class BreadCrumbDictionary
    {
        private static Dictionary<string, BreadCrumbItem[]> _dictionary;
        static BreadCrumbDictionary()
        {
            WriteDictionary();
        }
        private static void WriteDictionary()
        {
            _dictionary = new Dictionary<string, BreadCrumbItem[]>();
            _dictionary.Add("AdminEducationCategoryAdd",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Kategori Ekle", url: null) });
               _dictionary.Add("AdminEducationCategoryList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Kategori Listele", url: null) });

            _dictionary.Add("AdminHomeIndex",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Anasayfa", url: "/"), new BreadCrumbItem(title: "Admin Paneli", url: null) });
            _dictionary.Add("AdminEducatorAdd",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitmen Ekle", url: null) });
            _dictionary.Add("AdminEducatorList",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitmen Listele", url: null) });
        }

        public static BreadCrumbItem[] ReadPart(string part)
        {
            return _dictionary[part];
        }
    }
}
