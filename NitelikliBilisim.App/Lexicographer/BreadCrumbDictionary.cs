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
            _dictionary.Add("AdminEducationGrupDetail",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitim Grubu Listesi", url: "/admin/gruplar"), new BreadCrumbItem(title: "Eğitim Grubu Detayı", url: null) });
            _dictionary.Add("AdminEducationGrupEnterAttendance",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitim Grubu Listesi", url: "/admin/gruplar"), new BreadCrumbItem(title: "Grup Yoklaması", url: null) });
            _dictionary.Add("AdminEducationGroupLessonDayDetail",
            new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitim Grubu Listesi", url: "/admin/gruplar"), new BreadCrumbItem(title: "Eğitim Günü Detayları", url: null) });


            
            /*Educator Certificate*/
            _dictionary.Add("AdminEducatorCertificateList",
                new BreadCrumbItem[] {new BreadCrumbItem(title:"Admin Panel",url:"/admin/panel"),new BreadCrumbItem(title:"Eğitmen Sertifikası Listele",url:null)});
            _dictionary.Add("AdminEducatorCertificateAdd",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitmen Sertifika Listesi", url: "/admin/egitmensertifika/sertifikalar"), new BreadCrumbItem(title: "Eğitmen Sertifikası Ekle", url: null) });
            _dictionary.Add("AdminEducatorCertificateUpdate",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitmen Sertifika Listesi", url: "/admin/egitmensertifika/sertifikalar"), new BreadCrumbItem(title: "Eğitmen Sertifikası Güncelle", url: null) });

            /*Education Host*/
            _dictionary.Add("AdminEducationHostList",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitim Kurumları Listesi", url: null) });
            _dictionary.Add("AdminEducationHostAdd",
                new BreadCrumbItem[] { new BreadCrumbItem("Eğitim Kurumları Listesi", "/admin/egitim-kurumlari"), new BreadCrumbItem("Eğitim Kurumu Ekle", null) });
            _dictionary.Add("AdminEducationHostUpdate",
                new BreadCrumbItem[] { new BreadCrumbItem("Eğitim Kurumları Listesi", "/admin/egitim-kurumlari"), new BreadCrumbItem("Eğitim Kurumu Güncelle", null) });
            _dictionary.Add("AdminEducationClassRoomList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Sınıf Listesi", url: null) });


            
            /*OffDay Manage*/
            _dictionary.Add("AdminOffDay",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Tatil Günleri Yönetimi", url: null) });
            
            /*Education Suggestion Criterion*/
            _dictionary.Add("AdminEducationSuggestionCriterionManage",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Eğitimler Listesi", url: "/admin/egitimler"), new BreadCrumbItem(title: "Eğitim Öneri Kriterleri Yönetimi", url: null) });
            _dictionary.Add("AdminEducationSuggestionCriterionList",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Eğitim Öneri Kriterleri Listesi", url: null) });

            /*Student*/
            _dictionary.Add("AdminStudentList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Öğrenci Listele", url: null) });
            _dictionary.Add("AdminStudentLogList",
                 new BreadCrumbItem[] { new BreadCrumbItem(title: "Öğrenci Listesi", url: "/admin/ogrenci-yonetimi"), new BreadCrumbItem(title: "Öğrenci Hareketleri", url: null) });
            /*Blog*/
            _dictionary.Add("AdminBlogPostList",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Tüm Blog Yazıları", url: null) });
            _dictionary.Add("AdminBlogPostAdd",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Yeni Yazı Ekle", url: null) });
            _dictionary.Add("AdminBlogPostUpdate",
                new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Yazı Güncelle", url: null) });
            _dictionary.Add("AdminBlogPostView",
               new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Yazı Görüntüle", url: null) });
            /*Blog Category*/
            _dictionary.Add("AdminBlogCategoryList",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Tüm Blog Kategorileri", url: null) });
            _dictionary.Add("AdminBlogCategoryUpdate",
              new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Kategori Güncelle", url: null) });
            /* Blog Tag*/
            _dictionary.Add("AdminBlogTagList",
             new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Blog Etiket Listesi", url: null) });

            /*Expense*/
            _dictionary.Add("AdminExpenseTypeUpdate",
             new BreadCrumbItem[] { new BreadCrumbItem(title: "Admin Panel", url: "/admin/panel"), new BreadCrumbItem(title: "Grup Gideri Güncelle", url: null) });


        }

        public static BreadCrumbItem[] ReadPart(string part)
        {
            return _dictionary[part];
        }
    }
}
