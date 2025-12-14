using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record AnswerDetailDto(string QuestionText, string ChosenAnswer, bool IsCorrect);
