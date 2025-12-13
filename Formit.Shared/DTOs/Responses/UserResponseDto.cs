using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record UserResponseDto(
    string Id,
    string FullName,
    string Email,
    string UserName,
    IList<string> Roles
);
