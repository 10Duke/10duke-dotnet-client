using Tenduke.Client.Authorization;

namespace Tenduke.Client.Tests.Authorization;

public class AuthorizationCodeGrantArgsTests
{
    [Fact]
    public void WithNewCodeVerifierSetsVerifier()
    {
        var instance = new AuthorizationCodeGrantArgs();
        instance.WithNewCodeVerifier();
        Assert.False(string.IsNullOrEmpty(instance.CodeVerifier));
    }

    [Fact]
    public void ComputeCodeChallengeReturnsExpectedValue()
    {
        var instance = new AuthorizationCodeGrantArgs
        {
            CodeVerifier = "99DccPNbKI1E4NlAsbBcO06W_Yn~UlONrF4iYoyWMRq0D~RRjLGZO9-P2mTeL4Ih"
        };
        var result = instance.ComputeCodeChallenge();
        Assert.Equal("QUYHD2BztPSx9-Iw9QJQUF4JF6ypgDywhHWOqpPJ8x8", result);
    }
}
