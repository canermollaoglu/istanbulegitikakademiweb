using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.ViewModels.areas.admin.popular_topic;

namespace NitelikliBilisim.Business.Repositories
{
    public class PopularTopicRepository : BaseRepository<PopularTopic,Guid>
    {
        private readonly NbDataContext _context;
        public PopularTopicRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<PopularTopicListVm> GetListQueryable()
        {
            var data = _context.PopularTopics.Include(x => x.RelatedCategory)
                .Select(x => new PopularTopicListVm()
                {
                    Id = x.Id,
                    Title =x.Title,
                    ShortTitle = x.ShortTitle,
                    TargetUrl = x.TargetUrl,
                    RelatedCategory = x.RelatedCategory.Name
                });

            return data;

        }
    }
}
