using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGroupRepository : BaseRepository<EducationGroup, Guid>
    {
        private readonly NbDataContext _context;
        public EducationGroupRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public bool Insert(EducationGroup entity, List<int> days)
        {
            using (var transation = _context.Database.BeginTransaction())
            {
                try
                {
                    var daysJson = SerializeDays(days);
                    if (daysJson == null)
                        return false;

                    _context.EducationGroups.Add(entity);
                    _context.SaveChanges();
                    _context.GroupLessonDays.Add(new GroupLessonDays
                    {
                        DaysJson = daysJson,
                        GroupId = entity.Id
                    });
                    _context.SaveChanges();
                    transation.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    return false;
                }
            }
        }
        public EducationGroup GetFirstAvailableGroup(Guid educationId)
        {
            var group = _context.EducationGroups
                .Where(x => x.EducationId == educationId && x.IsGroupOpenForAssignment)
                .OrderBy(o => o.StartDate)
                .FirstOrDefault();

            return group;
        }

        private string SerializeDays(List<int> days)
        {
            if (days == null || days.Count == 0)
                return null;

            return JsonConvert.SerializeObject(days);
        }

    }
}
