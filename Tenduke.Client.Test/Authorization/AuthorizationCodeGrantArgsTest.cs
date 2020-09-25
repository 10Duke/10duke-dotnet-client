using NUnit.Framework;
using Tenduke.Client.Authorization;

namespace Tenduke.Client.Test.Authorization
{
    [TestFixture]
    public class AuthorizationCodeGrantArgsTest
    {
        [Test]
        public void TestWithNewCodeVerifier()
        {
            var instance = new AuthorizationCodeGrantArgs();
            instance.WithNewCodeVerifier();
            Assert.That(instance.CodeVerifier, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void TestComputeCodeChallenge()
        {
            var instance = new AuthorizationCodeGrantArgs();
            instance.CodeVerifier = "99DccPNbKI1E4NlAsbBcO06W_Yn~UlONrF4iYoyWMRq0D~RRjLGZO9-P2mTeL4Ih";
            var result = instance.ComputeCodeChallenge();
            Assert.That(result, Is.EqualTo("QUYHD2BztPSx9-Iw9QJQUF4JF6ypgDywhHWOqpPJ8x8"));
        }
    }
}
