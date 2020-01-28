using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
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
                    Console.WriteLine(ex.Message);
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

        public ListGetVm GetListVm()
        {
            var groups = _context.EducationGroups.Include(x => x.Education).Include(x => x.Host)
                .Select(x => new
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    StartDate = x.StartDate,
                    HostLocation = x.Host.City,
                    EducationName = x.Education.Name
                }).ToList();

            var data = new List<_Group>();
            foreach (var item in groups)
            {
                var hostLocation = EnumSupport.GetDescription(item.HostLocation);
                data.Add(new _Group
                {
                    GroupId = item.Id,
                    GroupName = item.GroupName,
                    EducationName = item.EducationName,
                    Location = hostLocation,
                    StartDate = item.StartDate
                });
            }

            return new ListGetVm
            {
                Groups = data
            };
        }
    }
}
