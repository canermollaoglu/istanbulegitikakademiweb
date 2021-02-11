using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    
    public class ContactFormController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public ContactFormController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-contact-forms")]
        public IActionResult GetContactForms(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.ContactForm.Get(x=>x.ContactFormType == Core.Enums.ContactFormTypes.ContactForm);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
        [HttpGet]
        [Route("get-faq-contact-forms")]
        public IActionResult GetFAQContactForms(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.ContactForm.Get(x => x.ContactFormType == Core.Enums.ContactFormTypes.SSS);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
