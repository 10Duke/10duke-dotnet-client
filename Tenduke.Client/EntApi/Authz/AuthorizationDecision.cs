using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Tenduke.Client.Util;

namespace Tenduke.Client.EntApi.Authz
{
    /// <summary>
    /// An authorization decision from the 10Duke Entitlement service.
    /// </summary>
    public class AuthorizationDecision
    {
        /// <summary>
        /// Authorization decision response as received from the server.
        /// </summary>
        public string RawResponse { get; set; }

        /// <summary>
        /// Payload of the authorization decision response as a dynamic object.
        /// </summary>
        public dynamic ResponseObject { get; set; }

        /// <summary>
        /// Gets an authorization decision field value.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The field value, may be <c>null</c>.</returns>
        public object this[string fieldName] { get { return ResponseObject[fieldName]?.Value; } }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>JSON string representing the authorization decision payload.</returns>
        public override string ToString()
        {
            return ResponseObject == null ? "" : JsonConvert.SerializeObject(ResponseObject);
        }

        /// <summary>
        /// Parses an authorization decision response and initializes a new instance of the
        /// <see cref="AuthorizationDecision"/> class.
        /// </summary>
        /// <param name="authorizedItem">Name of the item for which the authorization decision response is returned.</param>
        /// <param name="responseBody">Authorization decision response from the server, as a string.</param>
        /// <param name="contentType">The response content type.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying the token signature. If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response.</returns>
        /// <exception cref="SecurityTokenInvalidSignatureException">Thrown if the response is in JWT format and if token signature verification fails.</exception>
        public static AuthorizationDecision FromServerResponse(string authorizedItem, string responseBody, MediaTypeHeaderValue contentType, RSA verifyWithKey)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                throw new InvalidServerResponseException("The server returned an empty response");
            }

            switch (contentType.MediaType)
            {
                case "application/jwt":
                    return FromJwt(responseBody, verifyWithKey);
                case "application/json":
                    return FromJson(responseBody);
                default:
                    return FromPlainText(responseBody, authorizedItem);
            }
        }

        /// <summary>
        /// Parses an authorization decision response and returns a list of <see cref="AuthorizationDecision"/> objects.
        /// </summary>
        /// <param name="authorizedItems">Names of the items for which the authorization decision response is returned.</param>
        /// <param name="responseBody">Authorization decision response from the server, as a string.</param>
        /// <param name="contentType">The response content type.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying the token signature. If <c>null</c>, no verification is done.</param>
        /// <returns>List of <see cref="AuthorizationDecision"/> objects representing the authorization decision responses for the given
        /// <paramref name="authorizedItems"/>.</returns>
        /// <exception cref="SecurityTokenInvalidSignatureException">Thrown if the response is in JWT format and if token signature verification fails.</exception>
        public static IList<AuthorizationDecision> FromServerResponse(IList<string> authorizedItems, string responseBody, MediaTypeHeaderValue contentType, RSA verifyWithKey)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                throw new InvalidServerResponseException("The server returned an empty response");
            }

            if (authorizedItems == null)
            {
                return null;
            }

            if (authorizedItems.Count == 0)
            {
                return new List<AuthorizationDecision>(0);
            }

            var isJsonResponse = "application/json" == contentType.MediaType;
            if (isJsonResponse && authorizedItems.Count != 1)
            {
                throw new InvalidServerResponseException("JSON responses can only used for returning authorization decision for a single item");
            }

            var responseAuthzDecisionTokens = isJsonResponse ? new string[] { responseBody } : responseBody.Split('&');
            if (responseAuthzDecisionTokens.Length != authorizedItems.Count)
            {
                throw new InvalidServerResponseException(
                    "Expected response for "
                    + authorizedItems.Count
                    + " authorized items, but received "
                    + responseAuthzDecisionTokens.Length
                    + " response tokens");
            }

            switch (contentType.MediaType)
            {
                case "application/jwt":
                    return responseAuthzDecisionTokens.Select(token => FromJwt(token, verifyWithKey)).ToList();
                case "application/json":
                    var authorizationDecisionFromJson = FromJson(responseAuthzDecisionTokens[0]);
                    return new AuthorizationDecision[] { authorizationDecisionFromJson };
                default:
                    var authorizationDecisionsFromPlainText = new List<AuthorizationDecision>(authorizedItems.Count);
                    for (int i = 0; i < authorizedItems.Count; i++)
                    {
                        var authorizedItem = authorizedItems[i];
                        var responseToken = responseAuthzDecisionTokens[i];
                        var authorizationDecision = FromPlainText(responseToken, authorizedItem);
                        authorizationDecisionsFromPlainText.Add(authorizationDecision);
                    }
                    return authorizationDecisionsFromPlainText;
            }
        }

        /// <summary>
        /// Parses a JSON authorization decision response and initializes a new instance of the
        /// <see cref="AuthorizationDecision"/> class.
        /// </summary>
        /// <param name="jsonResponse">Authorization decision response from the server, as a JSON string.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response.</returns>
        public static AuthorizationDecision FromJson(string jsonResponse)
        {
            dynamic json = JsonConvert.DeserializeObject(jsonResponse);
            return new AuthorizationDecision()
            {
                RawResponse = jsonResponse,
                ResponseObject = json
            };
        }

        /// <summary>
        /// Parses a JWT authorization decision response and initializes a new instance of the
        /// <see cref="AuthorizationDecision"/> class.
        /// </summary>
        /// <param name="jwtResponse">Authorization decision response from the server, as a JWT string.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying the token signature. If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response.</returns>
        /// <exception cref="SecurityTokenInvalidSignatureException">Thrown if token signature verification fails.</exception>
        public static AuthorizationDecision FromJwt(string jwtResponse, RSA verifyWithKey)
        {
            var decoded = JwtUtil.ReadPayload(jwtResponse, verifyWithKey);
            dynamic json = JsonConvert.DeserializeObject(decoded);
            return new AuthorizationDecision()
            {
                RawResponse = jwtResponse,
                ResponseObject = json
            };
        }

        /// <summary>
        /// Parses a plain-text authorization decision response and initializes a new instance of the
        /// <see cref="AuthorizationDecision"/> class.
        /// </summary>
        /// <param name="plainTextResponse">Authorization decision response from the server in plain text, value being either "true" or "false".</param>
        /// <param name="authorizedItemName">Name of the item for which the authorization decision response is returned.</param>
        /// <returns><see cref="AuthorizationDecision"/> object representing the authorization decision response.</returns>
        public static AuthorizationDecision FromPlainText(string plainTextResponse, string authorizedItemName)
        {
            // Normalize the dynamic response object held by an AuthorizationDecision object by building and parsing
            // JSON representing the plain-text response
            string dummyJsonResponse = new StringBuilder()
                .Append("{\"").Append(authorizedItemName).Append("\": ").Append(plainTextResponse).Append("}").ToString();
            dynamic json = JsonConvert.DeserializeObject(dummyJsonResponse);
            return new AuthorizationDecision()
            {
                RawResponse = plainTextResponse,
                ResponseObject = json
            };
        }
    }
}
