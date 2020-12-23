using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class ContactFormController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public ContactFormController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult ContactForms()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminContactFormList");
            return View();
        }

        public IActionResult FAQForms()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminContactFAQFormList");
            return View();
        }

    }
}