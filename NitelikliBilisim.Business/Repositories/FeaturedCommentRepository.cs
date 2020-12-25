using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class FeaturedCommentRepository : BaseRepository<FeaturedComment,Guid>
    {
        private readonly NbDataContext _context;
        public FeaturedCommentRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public List<FeaturedCommentVm> GetFeaturedComments()
        {
            var data = _context.FeaturedComments.Select(x => new FeaturedCommentVm
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                Name = x.Name,
                Surname = x.Surname,
                Content = x.Content,
                Title = x.Title,
                PreviewVideo = x.FileUrl,
                PreviewImage = x.PreviewImageFileUrl
            }).OrderByDescending(x=>x.CreatedDate).ToList();

            return data;
        }
    }
}
