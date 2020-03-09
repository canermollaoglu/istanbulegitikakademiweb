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
            /*Home*/
            _dictionary.Add("AdminHomeIndex",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Anasayfa", url: "/"), new BreadCrumbItem(title: "Admin Paneli", url: null) });

            /*EducationCategory*/
            _dictionary.Add("AdminEducationCategoryAdd",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Kategori Listesi", url: "/admin/kategoriler"), new BreadCrumbItem(title: "Kategori Ekle", url: null) });
            _dictionary.Add("AdminEducationCategoryList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Kategori Listele", url: null) });
            _dictionary.Add("AdminEducationCategoryUpdate",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Kategori Listesi", url: "/admin/kategoriler"), new BreadCrumbItem(title: "Kategori Güncelle", url: null) });

            /*EducationTag*/
            _dictionary.Add("AdminEducationTagAdd",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitim Etiketleri Listesi", url: "/admin/etiketler"), new BreadCrumbItem(title: "Etiket Ekle", url: null) });
            _dictionary.Add("AdminEducationTagList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitim Etiketleri Listele", url: null) });
            _dictionary.Add("AdminEducationTagUpdate",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitim Etiketleri Listesi", url: "/admin/etiketler"), new BreadCrumbItem(title: "Etiket Güncelle", url: null) });

            /*Education*/
            _dictionary.Add("AdminEducationAdd",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Ekle", url: null) });
            _dictionary.Add("AdminEducationList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitimler Listele", url: null) });
            _dictionary.Add("AdminEducationUpdate",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Güncelle", url: null) });

            _dictionary.Add("AdminEducationMediaItem",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Medyaları", url: null) });
            _dictionary.Add("AdminEducationPart",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Parçaları", url: null) });
            _dictionary.Add("AdminEducationUpdatePart",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Parçaları Güncelle", url: null) });
            _dictionary.Add("AdminEducationGain",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Kazanımları", url: null) });
            _dictionary.Add("AdminEducationUpdateGain",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Kazanımları Güncelle", url: null) });
            _dictionary.Add("AdminEducationManageAssignEducators",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Eğitmenleri", url: null) });

            /*Educator*/
            _dictionary.Add("AdminEducatorAdd",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitmen Listesi", url: "/admin/egitmenler"), new BreadCrumbItem(title: "Eğitmen Ekle", url: null) });
            _dictionary.Add("AdminEducatorList",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitmen Listele", url: null) });
            _dictionary.Add("AdminEducatorUpdate",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitmen Listesi", url: "/admin/egitmenler"), new BreadCrumbItem(title: "Eğitmen Güncelle", url: null) });
            _dictionary.Add("AdminEducatorUpdateEducatorSocialMedia",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitmen Listesi", url: "/admin/egitmenler"), new BreadCrumbItem(title: "Eğitmen Sosyal Medya Güncelle", url: null) });

            /*EducationGrup*/
            _dictionary.Add("AdminEducationGrupList",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitim Grupları Listele", url: null) });
        }

        public static BreadCrumbItem[] ReadPart(string part)
        {
            return _dictionary[part];
        }
    }
}
