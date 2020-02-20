using Microsoft.AspNetCore.Identity;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.UoW
{
    public class UserUnitOfWork
    {
        private readonly NbDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private AppUserRepository _appUserRepository;
        private UserGroupRepository _userGroupRepository;
        public UserUnitOfWork(NbDataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public AppUserRepository User
        {
            get
            {
                return _appUserRepository ?? (_appUserRepository = new AppUserRepository(_context, _userManager));
            }
        }
        public UserGroupRepository Group
        {
            get
            {
                return _userGroupRepository ?? (_userGroupRepository = new UserGroupRepository(_context));
            }
        }
    }
}
