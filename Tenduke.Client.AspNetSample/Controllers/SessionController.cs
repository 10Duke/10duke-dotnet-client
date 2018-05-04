using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Tenduke.Client.AspNetSample.Controllers
{
    /// <summary>
    /// Controller for session actions via the API. Not used for standard authentication and authorization
    /// functionality of the sample application.
    /// </summary>
    public class SessionController : Controller
    {
        /// <summary>
        /// Explicitly triggers the authentication process.
        /// </summary>
        /// <returns>Returns a <see cref="ChallengeResult"/> for starting the authentication process.</returns>
        [HttpGet("~/signin")]
        public ActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/home" });
        }
    }
}
