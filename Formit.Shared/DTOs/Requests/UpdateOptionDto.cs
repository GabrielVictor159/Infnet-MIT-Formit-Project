using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class UpdateOptionDto
{
    public int Id { get; set; } = 0;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
