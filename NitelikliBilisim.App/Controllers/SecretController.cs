﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Core.Entities;

namespace NitelikliBilisim.App.Controllers
{
    public class SecretController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private const string SECRET_KEY = "gS10n1*_880";
        public SecretController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("secret")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("somewhere-over-the-rainbow")]
        public async Task<IActionResult> CreateAdmin(SecretHolder data)
        {
            if (data.secret != SECRET_KEY)
                return Json("");
            if (_userManager.Users.Any(x => x.Email == "admin@nbdev.com"))
                return Json("");

            var admin = new ApplicationUser
            {
                Email = "admin@nbdev.com",
                UserName = "admin",
                Name = "admin",
                Surname = "admin"
            };

            var result = await _userManager.CreateAsync(admin, "qwe123+Qwe123");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
            else return Json("");

            return Json("");
        }
    }

    public class SecretHolder
    {
        public string secret { get; set; }
    }
}