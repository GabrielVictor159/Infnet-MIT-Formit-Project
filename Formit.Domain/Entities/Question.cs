using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Domain.Entities;
public class Question
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;

    public byte[]? Image { get; set; }

    public int QuizId { get; set; }

    [ForeignKey("QuizId")]
    public Quiz Quiz { get; set; } = default!;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
