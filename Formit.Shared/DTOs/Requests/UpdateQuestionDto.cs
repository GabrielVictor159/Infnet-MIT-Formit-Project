using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class UpdateQuestionDto
{
    public int Id { get; set; } = 0;
    public string Text { get; set; } = string.Empty;
    public byte[]? Image { get; set; }

    public List<UpdateOptionDto> Options { get; set; } = new();
    public List<int> OptionsToDelete { get; set; } = new();
}