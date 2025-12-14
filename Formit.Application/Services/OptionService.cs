using Formit.Application.Interfaces;
using Formit.Domain.Entities;
using Formit.Infraestructure.Interfaces;
using Formit.Shared.DTOs;

namespace Formit.Application.Services;

public class OptionService : IOptionService
{
    private readonly IUnitOfWork _unitOfWork;

    public OptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OptionResponseDto> CreateAsync(int questionId, CreateOptionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
        if (question == null)
            throw new KeyNotFoundException($"Question with id {questionId} not found.");

        var option = new QuestionOption
        {
            QuestionId = questionId,
            OptionText = dto.OptionText,
            IsCorrect = dto.IsCorrect
        };

        await _unitOfWork.Options.AddAsync(option);
        await _unitOfWork.CompleteAsync();

        return new OptionResponseDto(option.Id, option.OptionText, option.IsCorrect);
    }

    public async Task<OptionResponseDto> UpdateAsync(int id, UpdateOptionDto dto)
    {
        var option = await _unitOfWork.Options.GetByIdAsync(id);
        if (option == null)
            throw new KeyNotFoundException($"Option with id {id} not found.");

        option.OptionText = dto.OptionText;
        option.IsCorrect = dto.IsCorrect;

        _unitOfWork.Options.Update(option);
        await _unitOfWork.CompleteAsync();

        return new OptionResponseDto(option.Id, option.OptionText, option.IsCorrect);
    }

    public async Task DeleteAsync(int id)
    {
        var option = await _unitOfWork.Options.GetByIdAsync(id);
        if (option == null)
            throw new KeyNotFoundException($"Option with id {id} not found.");

        _unitOfWork.Options.Remove(option);
        await _unitOfWork.CompleteAsync();
    }
}