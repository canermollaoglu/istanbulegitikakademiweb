using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.ViewModels.Main;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerCertificateRepository:BaseRepository<CustomerCertificate,Guid>
    {
        private readonly NbDataContext _context;
       public CustomerCertificateRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public VerifyCertificateVm VerifyCertificate(Guid certificateCode)
        {
            var data = (from customerCertificate in _context.CustomerCertificates
                       join user in _context.Users on customerCertificate.CustomerId equals user.Id
                       join eGroup in _context.EducationGroups on customerCertificate.GroupId equals eGroup.Id
                       join education in _context.Educations on eGroup.EducationId equals education.Id
                       where customerCertificate.Id == certificateCode
                       select new VerifyCertificateVm
                       {
                           StudentName = $"{user.Name} {user.Surname}",
                           EducationName = education.Name,
                           StartDate = eGroup.StartDate.ToString("dd MMMM yyyy"),
                           EndDate = eGroup.StartDate.AddDays(education.Days).ToString("dd MMMM yyyy"),
                           TotalHours = education.Days * education.HoursPerDay
                       }).FirstOrDefault();
            return data;
        }
    }

}
