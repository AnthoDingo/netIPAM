using Microsoft.AspNetCore.Authentication;
using netIPAM.Models.Interfaces;
using Okta.AspNetCore;

namespace netIPAM.Models.Provider
{
    public class Okta : IProvider
    {
        public string Domain { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;
        
        public string ClientSecret { get; set; } = string.Empty;
        
        public string AuthorizationServerId { get; set; } = string.Empty;

        public void Init(AuthenticationBuilder builder)
        {
            builder.AddOktaMvc(new OktaMvcOptions
            {
                OktaDomain = Domain,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                AuthorizationServerId = AuthorizationServerId,
            });
        }
    }
}
