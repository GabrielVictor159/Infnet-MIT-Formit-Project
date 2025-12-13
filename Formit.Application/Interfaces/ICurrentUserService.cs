using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Application.Interfaces;
public interface ICurrentUserService
{
    string Id { get; }
    string UserName { get; }
    string Email { get; }
    string FullName { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }

    string GetClaim(string claimType);
}
