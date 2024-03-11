using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tenduke.Client.Authorization;

/// <summary>
/// OAuth 2.0 Access Token.
/// </summary>
public class AccessTokenResponse
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = new SnakeCaseLowerJsonNamingPolicy(),
    };

    /// <summary>
    /// OpenID Connect ID token parsed from the access token response.
    /// </summary>
    private IDToken parsedIdToken;

    /// <summary>
    /// The access token issued by the 10Duke Entitlement service.
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// The type of the issued token.
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// The lifetime in seconds of the access token.
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// The refresh token, optional.
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// The scope of the access token, optional.
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// OpenID Connect ID token as a raw string (as returned in the OAuth response), optional.
    /// </summary>
    public string IdToken { get; set; }

    /// <summary>
    /// Indicates whether user would prefer this client application to store the access token response
    /// and use the same access token in subsequent client sessions.
    /// </summary>
    public bool? Remember { get; set; }

    /// <summary>
    /// RSA public key to use for verifying ID token signature.
    /// </summary>
    [JsonIgnore]
    public RSA SignerKey { get; set; }

    public string Error { get; set; }

    public string ErrorDescription { get; set; }

    public string ErrorUri { get; set; }

    /// <summary>
    /// Gets <see cref="IDToken"/> object for accessing values of the ID token received from the server.
    /// If signer key is specified, signature of the ID token is verified.
    /// </summary>
    [JsonIgnore]
    public IDToken IDTokenObject
    {
        get
        {
            if (parsedIdToken == null && IdToken!= null)
            {
                parsedIdToken = IDToken.Parse(IdToken, SignerKey);
            }

            return parsedIdToken;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class.
    /// </summary>
    public AccessTokenResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class by the given
    /// access token JSON response.
    /// </summary>
    /// <param name="json">Access token response as a JSON string.</param>
    /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
    /// If <c>null</c>, no verification is done.</param>
    /// <returns><see cref="AccessTokenResponse"/> object wrapping the access token response received from the server.</returns>
    public static AccessTokenResponse FromJsonStringResponse(string json, RSA verifyWithKey)
    {
        var response = JsonSerializer.Deserialize<AccessTokenResponse>(json, _jsonOptions);
        response.SignerKey = verifyWithKey;
        return response;
    }
}
