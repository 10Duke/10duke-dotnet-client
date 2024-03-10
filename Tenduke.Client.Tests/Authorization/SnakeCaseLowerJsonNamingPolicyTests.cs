using Tenduke.Client.Authorization;
namespace Tenduke.Client.Tests.Authorization;

public class SnakeCaseLowerJsonNamingPolicyTests
{
    [Theory]
    [InlineData("AccessToken", "access_token")]
    [InlineData("TokenType", "token_type")]
    [InlineData("ExpiresIn", "expires_in")]
    [InlineData("RefreshToken", "refresh_token")]
    [InlineData("Scope", "scope")]
    [InlineData("IdToken", "id_token")]
    [InlineData("Remember", "remember")]
    [InlineData("Error", "error")]
    [InlineData("ErrorDescription", "error_description")]
    [InlineData("ErrorUri", "error_uri")]
    public void ConvertNameConvertsIdTokenFieldsToExpectedValues(string input, string expected)
    {
        var converter = new SnakeCaseLowerJsonNamingPolicy();

        var result = converter.ConvertName(input);

        Assert.Equal(expected, result);
    }
}
