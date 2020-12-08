using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    public class BlogController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("blog/detail/{blogId}")]
        public IActionResult Detail(Guid? blogId)
        {
            if (!blogId.HasValue)
                return RedirectToAction("List");
            try
            {
                var model= _unitOfWork.BlogPost.GetPostById(blogId.Value);
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

    }
}
