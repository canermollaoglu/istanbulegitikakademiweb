using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Utility
{
    public static class CacheKeyUtility
    {
        public static string BeginnerEducations = "beginner-educations";
        public static string HeaderMenu = "header-menu";
        public static string PopularEducations = "popular-educations";
        public static string HomeNbuyCategories = "home-nbuy-categories";
        public static string HomeUserComments = "home-user-comments";
        public static string HomeEducationTags = "home-education-tags";
        public static string BlogLastPosts = "blog-last-posts";
        public static string BlogCategories = "blog-categories";

        public static List<string> GetAllCacheKeys() {
            var list = new List<string>();
            list.Add(BeginnerEducations);
            list.Add(HeaderMenu);
            list.Add(PopularEducations);
            list.Add(HomeNbuyCategories);
            list.Add(HomeUserComments);
            list.Add(HomeEducationTags);
            list.Add(BlogLastPosts);
            list.Add(BlogCategories);
            return list;
        
        }
    }

}
