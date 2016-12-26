using System;
using Microsoft.AspNetCore.Mvc;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class SchedulerController : Controller
    {
        [HttpGet("status")]
        public SchedulerStatus GetStatus()
        {
            return new SchedulerStatus() { QueuedTasks = 15 };
        }
    }

    public class SchedulerStatus
    {
        public int QueuedTasks { get; set; }
    }

}