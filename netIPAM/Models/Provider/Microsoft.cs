using Microsoft.AspNetCore.Authentication;
using netIPAM.Models.Interfaces;

namespace netIPAM.Models.Provider
{
    public class Microsoft : IProvider
    {
        public void Init(AuthenticationBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
