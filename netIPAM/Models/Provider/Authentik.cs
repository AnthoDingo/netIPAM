using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using netIPAM.Models.Interfaces;

namespace netIPAM.Models.Provider
{
    public class Authentik : IProvider
    {
        public string Domain { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
        
        public string ClientId { get; set; } = string.Empty;
        
        public string ClientSecret { get; set; } = string.Empty;

        private string Authority => $"{Domain}/application/o/{Slug}/";

        public void Init(AuthenticationBuilder builder)
        {
            builder.AddOpenIdConnect("Authentik", "Authentik", options =>
            {
                options.Authority = Authority;
                options.ClientId = ClientId;
                options.ClientSecret = ClientSecret;

                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "groups"
                };

                options.CallbackPath = "/signin-authentik";
            });
        }
    }
}
