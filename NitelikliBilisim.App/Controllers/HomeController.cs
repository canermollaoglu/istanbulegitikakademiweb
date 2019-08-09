using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;

namespace NitelikliBilisim.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Test, Guid> _testRepo;

        public HomeController(IRepository<Test, Guid> testRepo)
        {
            _testRepo = testRepo;
        }

        public IActionResult Index()
        {
            var first = _testRepo.GetAll().FirstOrDefault();
            if(first!= null)
            {
                first.UpdatedDate = DateTime.Now;
                _testRepo.Update(first);
            }

            return View(_testRepo.GetAll());
        }

        public IActionResult Privacy()
        {
            _testRepo.Add(new Test());
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
