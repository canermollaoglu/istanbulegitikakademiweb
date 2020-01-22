using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Data;
using System.Collections.Generic;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorSocialMediaRepository : BaseRepository<EducatorSocialMedia, int>
    {
        public EducatorSocialMediaRepository(NbDataContext context) : base(context)
        {
        }
        
        public void Insert(string educatorId, string facebook, string linkedin, string googlePlus, string twitter)
        {
            var list = new List<EducatorSocialMedia>();
            if (!string.IsNullOrWhiteSpace(facebook))
                list.Add(new EducatorSocialMedia
                {
                    EducatorId = educatorId,
                    SocialMediaType = EducatorSocialMediaType.Facebook,
                    Link = facebook
                });
            if (!string.IsNullOrWhiteSpace(linkedin))
                list.Add(new EducatorSocialMedia
                {
                    EducatorId = educatorId,
                    SocialMediaType = EducatorSocialMediaType.LinkedIn,
                    Link = linkedin
                });
            if (!string.IsNullOrWhiteSpace(googlePlus))
                list.Add(new EducatorSocialMedia
                {
                    EducatorId = educatorId,
                    SocialMediaType = EducatorSocialMediaType.GooglePlus,
                    Link = googlePlus
                });
            if (!string.IsNullOrWhiteSpace(twitter))
                list.Add(new EducatorSocialMedia
                {
                    EducatorId = educatorId,
                    SocialMediaType = EducatorSocialMediaType.Twitter,
                    Link = twitter
                });

            Context.AddRange(list);
            Context.SaveChanges();
        }
    }
}
