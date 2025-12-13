
using Microsoft.AspNetCore.Identity;

namespace Formit.Domain.Entities;
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
