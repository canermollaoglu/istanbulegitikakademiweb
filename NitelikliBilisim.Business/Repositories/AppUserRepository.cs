using Microsoft.AspNetCore.Identity;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class AppUserRepository
    {
        private readonly NbDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AppUserRepository(NbDataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
    }
}
