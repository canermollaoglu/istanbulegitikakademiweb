using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
