using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Tenduke.Client.AspNetSample.Controllers
{
    public class SessionController : Controller
    {
        [HttpGet("~/signin")]
        public ActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/home" });
        }
    }
}
