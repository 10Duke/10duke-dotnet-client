using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Tenduke.Client.AspNetCore;
using Tenduke.Client.AspNetCore.Config;
using Tenduke.Client.UserInfo;

namespace Tenduke.Client.AspNetSample.Controllers
{
    /// <summary>
    /// Controller for requesting sample data from the 10Duke Identity and Entitlement service.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        /// <summary>
        /// Gets user info from the <c>userinfo</c> endpoint.
        /// </summary>
        /// <returns><see cref="UserInfoData"/> object representing the user info.</returns>
        [HttpGet("[action]")]
        public async Task<UserInfoData> UserInfo()
        {
            var tendukeClient = await TendukeClient.BuildAsync(HttpContext);
            return await tendukeClient.UserInfoApi.GetUserInfoAsync();
        }
    }
}
