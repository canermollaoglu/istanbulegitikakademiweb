using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Filters;

namespace NitelikliBilisim.App.Controllers.Base
{
    [TypeFilter(typeof(UserLoggerFilterAttribute))]
    public class BaseController : Controller
    {
        
    }
}