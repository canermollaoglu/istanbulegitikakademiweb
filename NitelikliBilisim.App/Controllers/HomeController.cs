﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.App.Controllers
{
    public class HomeController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMessageService _emailService;
        public HomeController(UnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IMessageService emailService)
        {
            _roleManager = roleManager;
            _emailService = emailService;
            CheckRoles().Wait();
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexModel();
            var isLoggedIn = HttpContext.User.Identity.IsAuthenticated;
            if (!isLoggedIn)
                model.SuggestedEducations = _unitOfWork.Suggestion.SuggestEducationsForHomeIndex(false, null);
            else
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.SuggestedEducations = _unitOfWork.Suggestion.SuggestEducationsForHomeIndex(true, userId);
            }

            #region  test Email Service
            for (int i = 1; i <= 10; i++)
            {
                // Create a new message to send to the queue.
                var email = new EmailMessage(){
                    Subject = $"Subject {i}",
                    Body =$"Message Body {DateTime.Now:F}",
                    Contacts =  new []{"mesut.ozturk@wissenakademie.com"}
                };
              await _emailService.SendAsync(JsonConvert.SerializeObject(email));
            }

            #endregion

            return View(model);
        }


        [Route("yakinda")]
        public IActionResult ComingSoon()
        {
            return View();
        }

        //[Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        //[Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        async Task CheckRoles()
        {
            var roles = Enum.GetNames(typeof(IdentityRoleList));
            foreach (var item in roles)
            {
                if (await _roleManager.RoleExistsAsync(item) == false)
                {
                    var result = await _roleManager.CreateAsync(new ApplicationRole()
                    {
                        Name = item
                    });
                }
            }
        }
    }
}