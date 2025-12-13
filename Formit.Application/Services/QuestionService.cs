using Formit.Application.Interfaces;
using Formit.Domain.Entities;
using Formit.Infraestructure.Interfaces;
using Formit.Shared.DTOs;

namespace Formit.Application.Services;
public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;

    public QuestionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuestionResponseDto> GetByIdAsync(int id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            throw new KeyNotFoundException($"Question with id {id} not found.");

        var options = await _unitOfWork.Options.FindAsync(o => o.QuestionId == id);

        return new QuestionResponseDto(
            question.Id,
            question.Text,
            question.Image,
            options.Select(o => new OptionResponseDto(o.Id, o.OptionText)).ToList()
        );
    }

    public async Task<QuestionResponseDto> CreateAsync(int quizId, CreateQuestionDto dto)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with id {quizId} not found.");

        var question = new Question
        {
            QuizId = quizId,
            Image = dto.Image,
            Text = dto.Text
        };

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.CompleteAsync();

        if (dto.Options != null)
        {
            foreach (var optDto in dto.Options)
            {
                var option = new QuestionOption
                {
                    QuestionId = question.Id,
                    OptionText = optDto.OptionText,
                    IsCorrect = optDto.IsCorrect
                };
                await _unitOfWork.Options.AddAsync(option);
            }
            await _unitOfWork.CompleteAsync();
        }

        return await GetByIdAsync(question.Id);
    }

    public async Task<QuestionResponseDto> UpdateAsync(int id, UpdateQuestionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            throw new KeyNotFoundException($"Question with id {id} not found.");

        if(dto.Text != null)
            question.Text = dto.Text;

        question.Image = dto.Image;

        _unitOfWork.Questions.Update(question);
        await _unitOfWork.CompleteAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            throw new KeyNotFoundException($"Question with id {id} not found.");

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.CompleteAsync();
    }
}