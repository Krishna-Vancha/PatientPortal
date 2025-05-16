using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace PatientPortal.Controllers
{
    [Authorize(AuthenticationSchemes = "AzureAd")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello I am authenticated Via Azure Ad");
        }
    }
}
