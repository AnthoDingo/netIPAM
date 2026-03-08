using netIPAM.Models.Interfaces;
using System.Reflection;
using System.Linq;

namespace netIPAM.Models
{
    public class Providers
    {
        public Provider.Okta? Okta { get; set; }

        public Provider.Microsoft? EntraId { get; set; }

        public Provider.Authentik? Authentik { get; set; }

        public List<IProvider> GetAll()
        {
            PropertyInfo[] properties = this
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<IProvider> providers = properties
                .Select(p => p.GetValue(this))
                .OfType<IProvider>()
                .ToList();
            return providers;
        }
    }
}
