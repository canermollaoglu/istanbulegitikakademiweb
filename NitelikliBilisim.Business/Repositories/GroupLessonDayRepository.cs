﻿using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupLessonDayRepository : BaseRepository<GroupLessonDays, Guid>
    {
        private readonly NbDataContext _context;
        public GroupLessonDayRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public bool Insert(GroupLessonDays entity, List<int> days)
        {
            if (days == null || days.Count == 0)
                return false;

            var json = JsonConvert.SerializeObject(days);
            entity.DaysJson = json;
            _context.GroupLessonDays.Add(entity);
            _context.SaveChanges();

            return true;
        }
    }
}
