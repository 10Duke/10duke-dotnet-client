using Tenduke.Client.Authorization;

namespace Tenduke.Client.Tests.Authorization.AccessTokenResponseTests;

public class ErrorResponseTests
{
    private static readonly string ERROR_JSON = @"{
    ""error"": ""test-error-code"",
    ""error_description"": ""Description of test-error-code"",
    ""error_uri"": ""https://example.com/errors/test-error""
}";

    private readonly AccessTokenResponse _response;

    public ErrorResponseTests()
    {
        _response = AccessTokenResponse.FromJsonStringResponse(ERROR_JSON, null);
    }

    [Fact]
    public void FromJsonStringResponseSetsError()
    {
        Assert.Equal("test-error-code", _response.Error);
    }


    [Fact]
    public void FromJsonStringResponseSetsErrorDescription()
    {
        Assert.Equal("Description of test-error-code", _response.ErrorDescription);
    }

    [Fact]
    public void FromJsonStringResponseSetsErrorUri()
    {
        Assert.Equal("https://example.com/errors/test-error", _response.ErrorUri);
    }
}
