using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    [EnableCors("AllowAll")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
