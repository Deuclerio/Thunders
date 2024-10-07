using Microsoft.AspNetCore.Mvc;

namespace Thunders.Api.Controllers
{
    [Route("api/[controller]/")]    
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
    }
}
