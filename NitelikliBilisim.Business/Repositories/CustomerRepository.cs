using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.student;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, string>
    {
        private readonly NbDataContext _context;
        public CustomerRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable<Customer> GetCustomerListQueryable()
        {
            return Context.Customers.Include(x => x.User);

        }

        public StudentDetailVm GetCustomerDetail(string studentId)
        {
            var student = _context.Customers.Include(x => x.User)
                .Include(x=>x.Addresses).ThenInclude(x=>x.City)
                .Include(x=>x.Addresses).ThenInclude(x=>x.State)
                .First(x => x.Id == studentId);
            
            return new StudentDetailVm
            {
                Id = student.Id,
                RegistrationDate = student.CreatedDate,
                UserName = student.User.UserName,
                Name = student.User.Name,
                Surname = student.User.Surname,
                Email = student.User.Email,
                Phone = student.User.PhoneNumber,
                LinkedInProfile = student.LinkedInProfileUrl,
                Website = student.WebSiteUrl,
                DateOfBirth = student.DateOfBirth,
                IsNBUYStudent = student.IsNbuyStudent,
                AvatarPath = student.User.AvatarPath?? "/img/nb_letter_logo_sm.png",
                Addresses = student.Addresses,
                Job = student.Job
            };
        }
    }
}
