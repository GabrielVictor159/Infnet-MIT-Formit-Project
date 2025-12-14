using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Domain.Entities;
public class FormSubmission
{
    [Key]
    public int Id { get; set; }
    public int QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz Quiz { get; set; } = default!;

    [Required]
    public DateTime SubmissionDate { get; set; } = DateTime.Now;

    [StringLength(255)]
    public string? UserName { get; set; }
    public int Score { get; set; }

    public ICollection<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
}
