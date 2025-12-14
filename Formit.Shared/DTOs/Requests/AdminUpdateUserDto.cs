using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs.Requests;
public class AdminUpdateUserDto : UpdateUserDto
{
    [Required(ErrorMessage = "Role selection is required.")]
    [MinLength(1, ErrorMessage = "You must select at least one role.")]
    public List<string> Roles { get; set; } = new();
}
