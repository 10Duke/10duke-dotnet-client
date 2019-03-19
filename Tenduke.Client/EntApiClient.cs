using Tenduke.Client.Config;

namespace Tenduke.Client
{
    /// <summary>
    /// Client for calling APIs of the 10Duke Identity and Entitlement Service, specifically the
    /// <see cref="Tenduke.Client.EntApi.Authz.AuthzApi"/> and the <see cref="Tenduke.Client.UserInfo.UserInfoApi"/>.
    /// The API calls are authenticated and authorized by OAuth 2.0, i.e. <see cref="BaseClient{C, A}.AccessToken"/>
    /// must always be set for making API calls. 10Duke provides other platform specific client implementations
    /// derived from <see cref="BaseClient{C, A}"/> that also implement OAuth 2.0 authorization flow for acquiring
    /// the access token. This client implementation can be used when the access token is acquired using a custom
    /// process external to the client use case, or when no specific client implementation is available.
    /// </summary>
    public class EntApiClient : BaseClient<EntApiClient, OAuthConfig>
    {
    }
}
