using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record AuthResponseDto(
    bool Success,
    string Message,
    string? Token,
    string? UserName,
    IEnumerable<string>? Roles = null,
    IEnumerable<string>? Errors = null
);
