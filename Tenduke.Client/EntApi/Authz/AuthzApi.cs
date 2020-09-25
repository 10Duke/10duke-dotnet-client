using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.EntApi.Authz
{
    /// <summary>
    /// The <c>/authz/</c> authorization API of the 10Duke Entitlement service.
    /// </summary>
    public class AuthzApi : AuthorizedApi
    {
        #region Properties

        /// <summary>
        /// Configuration for accessing the <c>/authz/</c> API.
        /// </summary>
        public IAuthzApiConfig AuthzApiConfig { get; set; }

        /// <summary>
        /// Gets the identifier of this system.
        /// </summary>
        public string ComputerId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sends a request to the <c>/authz/</c> endpoint for getting an authorization decision. This authorization decision
        /// may only check for authorization, or it may consume a license.
        /// </summary>
        /// <param name="authorizedItem">Name of the item for which authorization is requested.</param>
        /// <param name="responseType">The <see cref="ResponseType"/> requested from the server, or <c>null</c> for server default.</param>
        /// <param name="consume"><c>true</c> to consume a license, <c>false</c> otherwise.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response from the server.</returns>
        public async Task<AuthorizationDecision> CheckOrConsumeAsync(string authorizedItem, bool consume = false, ResponseType responseType = null)
        {
            var authzDecisionRequestUri = BuildCheckOrConsumeUri(
                new string[] { authorizedItem },
                responseType,
                consume);
            var method = consume ? HttpMethod.Post : HttpMethod.Get;
            var responseData = await SendAuthorizationRequestAsync(authzDecisionRequestUri, method);
            var responseBody = responseData.Key;
            var responseContentType = responseData.Value;

            return AuthorizationDecision.FromServerResponse(authorizedItem, responseBody, responseContentType, AuthzApiConfig.SignerKey);
        }

        /// <summary>
        /// Sends a request to the <c>/authz/</c> endpoint for getting authorization decisions. The request
        /// may only check for authorizations, or it may consume licenses.
        /// </summary>
        /// <param name="authorizedItems">Names of the items for which authorization is requested.</param>
        /// <param name="responseType">The <see cref="ResponseType"/> requested from the server, or <c>null</c> for server default.</param>
        /// <param name="consume"><c>true</c> to consume a license, <c>false</c> otherwise.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response from the server.</returns>
        public async Task<IList<AuthorizationDecision>> CheckOrConsumeAsync(IList<string> authorizedItems, bool consume = false, ResponseType responseType = null)
        {
            var authzDecisionRequestUri = BuildCheckOrConsumeUri(
                authorizedItems,
                responseType,
                consume);
            var method = consume ? HttpMethod.Post : HttpMethod.Get;
            var responseData = await SendAuthorizationRequestAsync(authzDecisionRequestUri, method);
            var responseBody = responseData.Key;
            var responseContentType = responseData.Value;

            return AuthorizationDecision.FromServerResponse(authorizedItems, responseBody, responseContentType, AuthzApiConfig.SignerKey);
        }

        /// <summary>
        /// Releases a consumed license.
        /// </summary>
        /// <param name="consumptionId">The consumption id, as returned in the value of the <c>jti</c> authorization decision field.</param>
        /// <param name="responseType">The <see cref="ResponseType"/> requested from the server, or <c>null</c> for server default.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response from the server.</returns>
        public async Task<AuthorizationDecision> ReleaseLicenseAsync(string consumptionId, ResponseType responseType = null)
        {
            var releaseLicenseUri = BuildReleaseLicenseUri(new string[] { consumptionId }, responseType);
            var responseData = await SendAuthorizationRequestAsync(releaseLicenseUri, HttpMethod.Post);
            var responseBody = responseData.Key;
            var responseContentType = responseData.Value;

            return AuthorizationDecision.FromServerResponse(consumptionId, responseBody, responseContentType, AuthzApiConfig.SignerKey);
        }

        /// <summary>
        /// Releases consumed licenses.
        /// </summary>
        /// <param name="consumptionIds">The consumption ids, as returned in the value of the <c>jti</c> authorization decision field.</param>
        /// <param name="responseType">The <see cref="ResponseType"/> requested from the server, or <c>null</c> for server default.</param>
        /// <returns>List of <see cref="AuthorizationDecision"/> objects representing the authorization decision response from the server.</returns>
        public async Task<IList<AuthorizationDecision>> ReleaseLicensesAsync(IList<string> consumptionIds, ResponseType responseType = null)
        {
            var releaseLicenseUri = BuildReleaseLicenseUri(consumptionIds, responseType);
            var responseData = await SendAuthorizationRequestAsync(releaseLicenseUri, HttpMethod.Post);
            var responseBody = responseData.Key;
            var responseContentType = responseData.Value;

            return AuthorizationDecision.FromServerResponse(consumptionIds, responseBody, responseContentType, AuthzApiConfig.SignerKey);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Builds the full Uri for requesting authorization decisions for the given <paramref name="authorizedItems"/>.
        /// </summary>
        /// <param name="authorizedItems">Items for which authorization decisions are requested.</param>
        /// <param name="responseType">The response format to request.</param>
        /// <param name="consume"><c>true</c> to consume a license, <c>false</c> to only check if authorization is granted or denied.</param>
        /// <returns>Uri for the authorization decision request.</returns>
        protected Uri BuildCheckOrConsumeUri(IList<string> authorizedItems, ResponseType responseType, bool consume)
        {
            if (AuthzApiConfig == null)
            {
                throw new InvalidOperationException("AuthzApiConfig must be specified");
            }

            if (AuthzApiConfig.EndpointUri == null)
            {
                throw new InvalidOperationException("AuthzApiConfig.EndpointUri must be specified");
            }

            var uriBuilder = new UriBuilder(AuthzApiConfig.EndpointUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var authorizedItem in authorizedItems)
            {
                query[authorizedItem] = null;
            }

            query["doConsume"] = consume ? "true" : "false";

            if (ComputerId != null)
            {
                query["hw"] = ComputerId;
            }

            if (responseType != null)
            {
                uriBuilder.Path += responseType.Extension;
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Builds the full Uri for requesting a license release.
        /// </summary>
        /// <param name="consumptionIds">The consumption ids, as returned in the value of the <c>jti</c> authorization decision field.</param>
        /// <param name="responseType">The response format to request.</param>
        /// <returns>Uri for the license release request.</returns>
        protected Uri BuildReleaseLicenseUri(IList<string> consumptionIds, ResponseType responseType)
        {
            if (AuthzApiConfig == null)
            {
                throw new InvalidOperationException("AuthzApiConfig must be specified");
            }

            if (AuthzApiConfig.EndpointUri == null)
            {
                throw new InvalidOperationException("AuthzApiConfig.EndpointUri must be specified");
            }

            var uriBuilder = new UriBuilder(AuthzApiConfig.EndpointUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["release"] = null;
            foreach (var consumptionId in consumptionIds)
            {
                query[consumptionId] = null;
            }

            if (responseType != null)
            {
                uriBuilder.Path += responseType.Extension;
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Sends a request to the <c>/authz/</c> endpoint, either for getting an authorization decision or for releasing a license.
        /// </summary>
        /// <param name="uri">The request Uri.</param>
        /// <param name="method">The request HTTP method.</param>
        /// <returns><see cref="KeyValuePair{TKey, TValue}"/> object where key is the response body, value is the response content type.</returns>
        private async Task<KeyValuePair<string, MediaTypeHeaderValue>> SendAuthorizationRequestAsync(Uri uri, HttpMethod method)
        {
            if (AccessToken == null)
            {
                throw new InvalidOperationException("AccessToken must be specified");
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = method
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            request.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true
            };
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContentType = response.Content.Headers.ContentType;
            var responseBody = await response.Content.ReadAsStringAsync();

            return new KeyValuePair<string, MediaTypeHeaderValue>(responseBody, responseContentType);
        }

        #endregion
    }
}
