using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Filters;

namespace NitelikliBilisim.App.Controllers.Base
{

    [Authorize]
    [TypeFilter(typeof(HandleExceptionAttribute))]
    public class BaseController : Controller
    {
        
    }
}