using System.IdentityModel.Tokens.Jwt;
using Alba;
using IdentityModel;

namespace IdentityServer.Tests;

[TestFixture]
public class GetToken
{
    [Test]
    public async Task UseTokenForRequest()
    {
        var result = await Application.Host.Scenario(s =>
        {
            var data = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", "test"},
                {"client_secret", "test"},
                {"scope", Constants.Systemscope}
            };

            s.Post.FormData(data)
                .ToUrl("/connect/token");
            s.StatusCodeShouldBeOk();
        });

        var payload = await result.ReadAsJsonAsync<Token>();
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(payload!.access_token);

        Assert.That(token.Issuer, Is.EqualTo("http://localhost"));
        Assert.That(token.Claims.First(c => c.Type == JwtClaimTypes.AuthorizedParty).Value, Is.EqualTo(Constants.Clientid));
        Assert.That(token.Claims.First(c => c.Type == "aud").Value, Is.EqualTo(Constants.Audience));
        Assert.That(token.Claims.First(c => c.Type == "scp").Value, Is.EqualTo(Constants.Systemscope));
    }

    class Token
    {
        public string access_token { get; set; }
    }
}