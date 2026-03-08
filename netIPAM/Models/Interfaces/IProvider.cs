using Microsoft.AspNetCore.Authentication;

namespace netIPAM.Models.Interfaces
{
    public interface IProvider
    {
        public void Init(AuthenticationBuilder builder);
    }
}
