using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class CreateOptionDto
{
    [Required]
    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
