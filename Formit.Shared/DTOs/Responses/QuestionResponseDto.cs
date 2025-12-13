using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Shared.DTOs;
public record QuestionResponseDto(
    int Id,
    string Text,
    byte[]? Image,
    List<OptionResponseDto> Options
);