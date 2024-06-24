using System.Security.Claims;
using IdentityModel;
using IdentityServer;
using IdentityServer4.Models;
using IdentityServer4.Validation;

//Devtunnels
var devtunnelUri = Environment.GetEnvironmentVariable("VS_TUNNEL_URL");
var devTunnelEnabled = !string.IsNullOrWhiteSpace(devtunnelUri);
if(devTunnelEnabled) Console.WriteLine($"Using dev tunnel at: {devtunnelUri}");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityServer(a =>
    {
        a.IssuerUri = devTunnelEnabled ? devtunnelUri!.TrimEnd('/') : null;
        a.EmitScopesAsSpaceDelimitedStringInJwt = true;
        a.AccessTokenJwtType = "JWT";        
    })
    .AddDeveloperSigningCredential()
    .AddJwtBearerClientAuthentication()
    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
    .AddInMemoryApiScopes([
        new (Constants.Systemscope, "SMART System read"),
        new (Constants.Userscope, "SMART Patient read")
    ])
    .AddInMemoryApiResources([new(Constants.Audience, "SMART") {
        Scopes = [Constants.Systemscope, Constants.Userscope]
    }])
    .AddInMemoryClients([
        new Client{
            ClientId = "test",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = [new Secret("test".Sha256())],
            AllowedScopes = [Constants.Systemscope, Constants.Userscope],
            Claims = [
                new (JwtClaimTypes.AuthorizedParty, Constants.Clientid)
            ],
            ClientClaimsPrefix = ""
        }
    ]);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseIdentityServer();

app.Run();

public class CustomTokenRequestValidator : ICustomTokenRequestValidator
{
    public Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
        if (context.Result.ValidatedRequest.RequestedScopes.Contains(Constants.Userscope))
        {
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("fhirUser", "https://example.org/Practitioner/123456798"));
        }

        //Rename 'scope' to 'scp' to follow the documentation
        context.Result.ValidatedRequest.ClientClaims.Add(new("scp",
            string.Join(' ', context.Result.ValidatedRequest.ValidatedResources.ParsedScopes.Select(s => s.RawValue))));
        context.Result.ValidatedRequest.ValidatedResources.ParsedScopes.Clear();
        
        return Task.CompletedTask;
    }
}