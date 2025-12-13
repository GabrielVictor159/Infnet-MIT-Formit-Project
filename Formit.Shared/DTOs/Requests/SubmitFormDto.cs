using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class SubmitFormDto()
{
    [Required] 
    public int QuizId { get; set; }
    public string? UserName { get; set; }
    public List<SubmitAnswerDto> Answers { get; set; } = new();
}
