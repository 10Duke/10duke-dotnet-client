using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tenduke.Client.AspNetSample.Controllers
{
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        [HttpGet("[action]")]
        public RedirectResult Login()
        {
            var r = Res;
            return Redirect("https://vslidp.10duke.com");
        }
    }
}
