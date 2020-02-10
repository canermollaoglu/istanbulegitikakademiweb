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
                .Include(x => x.Host)
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
                .Include(x => x.GroupStudents)
                .Select(x => new
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    StartDate = x.StartDate,
                    HostLocation = x.Host.City,
                    EducationName = x.Education.Name,
                    Quota = x.Quota,
                    AssignedCount = x.GroupStudents.Count
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
                    StartDate = item.StartDate,
                    AssignedCount = item.AssignedCount,
                    Quota = item.Quota
                });
            }

            return new ListGetVm
            {
                Groups = data
            };
        }

        //public GroupAssignmentGetVm GetEligibleTicketsToAssign(Guid groupId)
        //{
        //    var group = _context.EducationGroups.FirstOrDefault(x => x.Id == groupId);
        //    if (group == null)
        //        return null;
        //    if (!group.IsGroupOpenForAssignment)
        //        return null;

        //    var eligibleTickets = _context.Tickets
        //        .Include(x => x.Owner)
        //        .Where(x => !x.IsUsed && x.EducationId == group.EducationId)
        //        .ToList();

        //    var groupTickets = _context.Bridge_GroupStudents
        //        .Where(x => x.Id == groupId)
        //        .Include(x => x.Ticket)
        //        .ThenInclude(x => x.Owner)
        //        .ToList();

        //    return new GroupAssignmentGetVm
        //    {
        //        AssignedStudents = groupTickets,
        //        EligibleTickets = eligibleTickets
        //    };
        //}
    }
}
