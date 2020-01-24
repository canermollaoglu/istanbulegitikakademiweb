using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NitelikliBilisim.App.Filters
{
    public class ComingSoonActionFilter : IActionFilter
    {
        private bool redirected = false;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!redirected)
                context.Result = new RedirectResult("/yakinda");

            redirected = true;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            redirected = false;
        }
    }
}
