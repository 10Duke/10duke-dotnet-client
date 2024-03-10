using Tenduke.Client.Config;
using Tenduke.Client.EntApi;
using Tenduke.Client.Util;

namespace Tenduke.Client.Tests;

public class EntApiClientTests
{
    private readonly string accessToken = "insert-valid-oauth-access-token-value-here";
    private readonly string userInfoUri = "https://my-ent.10duke.net/userinfo";
    private readonly string authzUri = "https://my-ent.10duke.net/authz/";
    // Replace this with the environment specific PEM / PKCS#1 encoded RSA key
    private readonly string tokenSignerKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxSjO1Dmx3ubqPl13Ch60
K9qiG0xyuEUSes2ZhzvwsEHu3q5vTuewNzFqZe3g67lz5zfbzjAkcMKMJ6CjxwgL
b2Zt70DOuXUNT7o6JJ9H7HycNAnMmX2Luhv/9JrULNqdMpO0n1ncEsZVcW/cZH+t
tFT2ssc7kISdokLJdDp4CEhUEXu23Zzyxw+vmOUpbYxsUIw4nmhLihxRtTP5WwhY
smbVXi0S3sbSrkUyisotf94ZMSbiFb5gnYEO4kc5FlyXET8nqL1ck++l5oyK1tLO
bPqW6l/8gsDUzpf6hQaQP3/NTLFX4T3sBSkX55MDtnEnQePwylfJXxmKGkjapCeX
FQIDAQAB
-----END PUBLIC KEY-----";

    [Fact(Skip = "UserInfoUri and AccessToken must be configured for running the test")]
    public async Task CanGetUserInfo()
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

        Assert.False(string.IsNullOrEmpty(userInfoData.sub));
    }

    [Fact(Skip = "SignerKey, EndpointUri and AccessToken must be configured for running the test")]
    public async Task CanCheckAccessToLicensedItem()
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

        var authzApi = client.AuthzApi;
        var computerId = "MyComputerId";
        authzApi.ComputerId = computerId;

        var authorizationDecision = await authzApi.CheckOrConsumeAsync("MyTestItem", false, ResponseType.JWT);

        Assert.False(string.IsNullOrEmpty(authorizationDecision["iss"].ToString()));
        Assert.Equal(computerId, authzApi.ComputerId);
    }
}
