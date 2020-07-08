using NitelikliBilisim.Core.Entities.helper;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class OffDayRepository :BaseRepository<OffDay,int>
    {
        private readonly NbDataContext _context;
        public OffDayRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Kayıt tekrarı varsa True döner.
        /// </summary>
        /// <param name="date">Tatil günü</param>
        /// <returns></returns>
        public bool IsDuplicate(int day,int month,int year)
        {
            var temp = _context.OffDays.FirstOrDefault(x=>x.Day == day && x.Month == month && x.Year == year );
            if (temp != null)
            {
                return true;
            }
            return false;
        }

    }
}
