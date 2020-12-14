using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Components
{
    public class RecommendedBlogPosts : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public RecommendedBlogPosts(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke(Guid currentBlogPostId)
        {
            return View(_unitOfWork.BlogPost.GetUserRecommendedBlogPosts(currentBlogPostId));
        }
    
    }
}
