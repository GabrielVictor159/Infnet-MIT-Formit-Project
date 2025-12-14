using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record SubmissionDetailsDto(int Id, string? UserName, DateTime SubmissionDate, int Score, List<AnswerDetailDto> Answers);
