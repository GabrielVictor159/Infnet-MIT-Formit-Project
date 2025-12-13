using Formit.Shared.DTOs;

namespace Formit.App.Services.Interfaces;

public interface IOptionService
{
    Task<OptionResponseDto> CreateAsync(int questionId, CreateOptionDto dto);
    Task<OptionResponseDto> UpdateAsync(int id, UpdateOptionDto dto);
    Task DeleteAsync(int id);
}
