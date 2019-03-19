using NUnit.Framework;
using System.Threading.Tasks;
using Tenduke.Client.Config;
using Tenduke.Client.EntApi;
using Tenduke.Client.Util;

namespace Tenduke.Client
{
    public class EntApiClientTest
    {
        private readonly string accessToken = "insert-valid-oauth-access-token-value-here";
        private readonly string userInfoUri = "https://my-ent.10duke.net/userinfo";
        private readonly string authzUri = "https://my-ent.10duke.net/authz/";
        private readonly string tokenSignerKey = @"Insert PEM / PKCS#1 encoded RSA key here";

        [Test]
        [Ignore("UserInfoUri and AccessToken must be configured for running the test")]
        public async Task TestUserInfo()
        {
            var oauthConfig = new OAuthConfig()
            {
                UserInfoUri = userInfoUri
            };

            var client = new EntApiClient()
            {
                OAuthConfig = oauthConfig,
                AccessToken = accessToken
            };

            var userInfoData = await client.UserInfoApi.GetUserInfoAsync();

            Assert.That(userInfoData.sub, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Ignore("SignerKey, EndpointUri and AccessToken must be configured for running the test")]
        public async Task TestAuthzApi()
        {
            var signerKey = CryptoUtil.ReadRsaPublicKey(tokenSignerKey);
            var authzApiConfig = new AuthzApiConfig()
            {
                EndpointUri = authzUri,
                SignerKey = signerKey
            };

            var client = new EntApiClient()
            {
                AuthzApiConfig = authzApiConfig,
                AccessToken = accessToken
            };

            var authorizationDecision = await client.AuthzApi.CheckOrConsumeAsync("MyTestItem", false, ResponseType.JWT);

            Assert.That(authorizationDecision["iss"], Is.Not.Null.And.Not.Empty);
        }
    }
}