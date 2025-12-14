using Formit.Shared.DTOs;

namespace Formit.App.Services.Interfaces;

public interface IQuizService
{
    Task<PagedResultDto<QuizResponseDto>> GetAllPagedAsync(int page, int pageSize, string? title = null);

    Task<QuizResponseDto?> GetByIdAsync(int id);

    Task<QuizResponseDto> CreateAsync(CreateQuizDto dto);

    Task<QuizResponseDto> UpdateAsync(int id, UpdateQuizDto dto);

    Task DeleteAsync(int id);
}
