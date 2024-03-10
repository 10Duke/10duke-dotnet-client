using Tenduke.Client.Authorization;

namespace Tenduke.Client.Tests.Authorization.AccessTokenResponseTests;

public class SuccessResponseTests
{
    private static readonly string SUCCESS_JSON = @"{
    ""access_token"": ""test-access-token"",
    ""token_type"": ""Bearer"",
    ""expires_in"": 3600,
    ""refresh_token"": ""test-refresh-token"",
    ""scope"": ""email profile"",
    ""id_token"": ""test-id-token"",
    ""remember"": true
}";

    private readonly AccessTokenResponse _response;

    public SuccessResponseTests()
    {
        _response = AccessTokenResponse.FromJsonStringResponse(SUCCESS_JSON, null);
    }

    [Fact]
    public void FromJsonStringResponseSetsAccessToken()
    {
        Assert.Equal("test-access-token", _response.AccessToken);
    }

    [Fact]
    public void FromJsonStringResponseSetsTokenType()
    {
        Assert.Equal("Bearer", _response.TokenType);
    }

    [Fact]
    public void FromJsonStringResponseSetsExpiresIn()
    {
        Assert.Equal(3600, _response.ExpiresIn);
    }

    [Fact]
    public void FromJsonStringResponseSetsRefreshToken()
    {
        Assert.Equal("test-refresh-token", _response.RefreshToken);
    }

    [Fact]
    public void FromJsonStringResponseSetsScope()
    {
        Assert.Equal("email profile", _response.Scope);
    }

    [Fact]
    public void FromJsonStringResponseSetsIdToken()
    {
        Assert.Equal("test-id-token", _response.IdToken);
    }

    [Fact]
    public void FromJsonStringResponseSetsIdRemember()
    {
        Assert.Equal(true, _response.Remember);
    }
}
