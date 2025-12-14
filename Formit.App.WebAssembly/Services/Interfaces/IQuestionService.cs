using Formit.Shared.DTOs;

namespace Formit.App.Services.Interfaces;

public interface IQuestionService
{
    Task<QuestionResponseDto?> GetByIdAsync(int id);
    Task<QuestionResponseDto> CreateAsync(int quizId, CreateQuestionDto dto);
    Task<QuestionResponseDto> UpdateAsync(int id, UpdateQuestionDto dto);
    Task DeleteAsync(int id);
}
