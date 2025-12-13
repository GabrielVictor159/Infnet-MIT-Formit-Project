using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Domain.Entities;
public class QuestionOption
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }

    [ForeignKey("QuestionId")]
    public Question Question { get; set; } = default!;
}
