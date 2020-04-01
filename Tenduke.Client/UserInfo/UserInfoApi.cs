using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.UserInfo
{
    /// <summary>
    /// The <c>/userinfo</c> API of the 10Duke Entitlement service.
    /// </summary>
    public class UserInfoApi : AuthorizedApi
    {
        #region Properties

        /// <summary>
        /// Configuration for accessing the <c>/userinfo</c> API.
        /// </summary>
        public IOAuthConfig OAuthConfig { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the user info.
        /// </summary>
        /// <returns><see cref="UserInfoData"/> object representig the queried user info</returns>
        public async Task<UserInfoData> GetUserInfoAsync()
        {
            if (OAuthConfig == null)
            {
                throw new InvalidOperationException("OAuthConfig must be specified");
            }

            var userInfoEndpointUri = OAuthConfig.UserInfoUri;
            if (string.IsNullOrEmpty(userInfoEndpointUri))
            {
                throw new InvalidOperationException("UserInfoUri must be specified in OAuthConfig");
            }

            var accessToken = AccessToken;
            if (accessToken == null)
            {
                throw new InvalidOperationException("AccessToken must be specified");
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(OAuthConfig.UserInfoUri),
                Method = HttpMethod.Get
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true
            };
            using (var response = await HttpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<UserInfoData>();
            }
        }

        #endregion
    }
}
