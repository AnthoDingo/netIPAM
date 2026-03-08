using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Models.Account
{
    [Table("Roles")]
    public class AppRole : IdentityRole
    {
    }
}
