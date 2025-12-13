using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record SubmissionSummaryDto(int Id, string? UserName, DateTime SubmissionDate, int Score);
