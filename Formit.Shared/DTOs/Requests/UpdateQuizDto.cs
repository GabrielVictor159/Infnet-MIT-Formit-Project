using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public class UpdateQuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[]? Image { get; set; }
    public List<UpdateQuestionDto> Questions { get; set; } = new();
    public List<int> QuestionsToDelete { get; set; } = new();
}
