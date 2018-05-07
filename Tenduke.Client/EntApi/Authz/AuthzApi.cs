using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Tenduke.Client.Authorization;
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
        public AuthorizationDecision CheckOrConsume(string authorizedItem, bool consume = false, ResponseType responseType = null)
        {
            var authzDecisionRequestUri = BuildCheckOrConsumeUri(
                new string[] { authorizedItem },
                responseType,
                consume);
            var method = consume ? "POST" : "GET";
            var responseData = SendAuthorizationRequest(authzDecisionRequestUri, method);
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
        public IList<AuthorizationDecision> CheckOrConsume(IList<string> authorizedItems, bool consume = false, ResponseType responseType = null)
        {
            var authzDecisionRequestUri = BuildCheckOrConsumeUri(
                authorizedItems,
                responseType,
                consume);
            var method = consume ? "POST" : "GET";
            var responseData = SendAuthorizationRequest(authzDecisionRequestUri, method);
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
        public AuthorizationDecision ReleaseLicense(string consumptionId, ResponseType responseType = null)
        {
            var releaseLicenseUri = BuildReleaseLicenseUri(new string[] { consumptionId }, responseType);
            var responseData = SendAuthorizationRequest(releaseLicenseUri, "POST");
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
        public IList<AuthorizationDecision> ReleaseLicenses(IList<string> consumptionIds, ResponseType responseType = null)
        {
            var releaseLicenseUri = BuildReleaseLicenseUri(consumptionIds, responseType);
            var responseData = SendAuthorizationRequest(releaseLicenseUri, "POST");
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
        private KeyValuePair<string, string> SendAuthorizationRequest(Uri uri, string method)
        {
            if (AccessToken == null)
            {
                throw new InvalidOperationException("AccessToken must be specified");
            }

            var tokenRequest = WebRequest.CreateHttp(uri);
            tokenRequest.Method = method;
            tokenRequest.AllowAutoRedirect = false;
            tokenRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AccessToken);

            string responseBody;
            string responseContentType;
            using (var response = tokenRequest.GetResponse())
            {
                responseContentType = new System.Net.Mime.ContentType(response.ContentType).MediaType;
                using (var responseStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseBody = responseStreamReader.ReadToEnd();
                }
            }

            return new KeyValuePair<string, string>(responseBody, responseContentType);
        }

        #endregion
    }
}
