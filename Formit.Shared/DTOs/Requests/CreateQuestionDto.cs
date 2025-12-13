using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class CreateQuestionDto
{
    [Required]
    public string Text { get; set; } = string.Empty;

    public byte[]? Image { get; set; }

    public List<CreateOptionDto> Options { get; set; } = new();
}
