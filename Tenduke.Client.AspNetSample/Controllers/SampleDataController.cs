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
using Tenduke.Client.AspNetCore.Config;

namespace Tenduke.Client.AspNetSample.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        [HttpGet("[action]")]
        public async Task<UserInfoData> UserInfo()
        {
            UserInfoData retValue = null;

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var tendukeOAuthConfig = DefaultConfiguration.LoadOAuthConfiguration();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(tendukeOAuthConfig.UserInfoUri);
                retValue = await response.Content.ReadAsAsync<UserInfoData>();
            }

            return retValue;
        }

        public class UserInfoData
        {
            // OpenID Connect scope "profile"
#pragma warning disable IDE1006 // Naming Styles
            public string sub { get; set; }
            public string name { get; set; }
            public string nickname { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string gender { get; set; }
            public string birthdate { get; set; }
            public string website { get; set; }

            // OpenID Connect scope "email"
            public string email { get; set; }

            // OpenID Connect scope "address"
            public Address address { get; set; }

            // OpenID Connect scope "phone"
            public string phone_number { get; set; }

            // OpenID Connect scope "organization"
            public Organization organization { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Address
        {
#pragma warning disable IDE1006 // Naming Styles
            public string street_address { get; set; }
            public string locality { get; set; }
            public string region { get; set; }
            public string postal_code { get; set; }
            public string country { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Organization
        {
#pragma warning disable IDE1006 // Naming Styles
            public string id { get; set; }
            public string name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}
