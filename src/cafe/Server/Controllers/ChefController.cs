using Microsoft.AspNetCore.Mvc;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class ChefController : Controller
    {
        [HttpGet("run")]
        public TaskStatus RunChef()
        {
            return null;
        }

        [HttpGet("install")]
        public TaskStatus InstallChef()
        {
            return null;
        }

        [HttpGet("download")]
        public TaskStatus DownloadChef(string version)
        {
            return null;
        }
    }
}