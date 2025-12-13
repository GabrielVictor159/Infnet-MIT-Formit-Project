using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Domain.Entities;
public class QuestionResponse
{
    [Key]
    public int Id { get; set; }

    public int FormSubmissionId { get; set; }

    [ForeignKey("FormSubmissionId")]
    public FormSubmission FormSubmission { get; set; } = default!;

    public int QuestionId { get; set; }

    [ForeignKey("QuestionId")]
    public Question Question { get; set; } = default!;

    public int ChosenOptionId { get; set; }

    [ForeignKey("ChosenOptionId")]
    public QuestionOption ChosenOption { get; set; } = default!;
}

