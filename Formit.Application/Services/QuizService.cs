using Formit.Application.Interfaces;
using Formit.Domain.Entities;
using Formit.Infraestructure.Interfaces;
using Formit.Shared.DTOs;

namespace Formit.Application.Services;
public class QuizService : IQuizService
{
    private readonly IUnitOfWork _unitOfWork;

    public QuizService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<QuizResponseDto>> GetAllPagedAsync(int page, int pageSize, string? title)
    {
        System.Linq.Expressions.Expression<Func<Quiz, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(title))
        {
            filter = q => q.Title.Contains(title);
        }

        var pagedQuizzes = await _unitOfWork.Quizzes.GetPagedAsync(page, pageSize, filter);

        var quizDtos = pagedQuizzes.Items.Select(q => new QuizResponseDto(
            q.Id,
            q.Title,
            q.Description,
            q.Image,
            new List<QuestionResponseDto>() 
        ));

        return new PagedResultDto<QuizResponseDto>
        {
            Items = quizDtos,
            TotalCount = pagedQuizzes.TotalCount,
            PageNumber = pagedQuizzes.PageNumber,
            PageSize = pagedQuizzes.PageSize
        };
    }

    public async Task<IEnumerable<QuizResponseDto>> GetAllAsync()
    {
        var quizzes = await _unitOfWork.Quizzes.GetAllAsync();

        return quizzes.Select(q => new QuizResponseDto(
            q.Id,
            q.Title,
            q.Description,
            q.Image,
            new List<QuestionResponseDto>()
        ));
    }

    public async Task<QuizResponseDto> GetByIdAsync(int id)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id);
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with id {id} not found.");

        var questions = await _unitOfWork.Questions.FindAsync(q => q.QuizId == id);

        var questionDtos = new List<QuestionResponseDto>();

        foreach (var q in questions)
        {
            var options = await _unitOfWork.Options.FindAsync(o => o.QuestionId == q.Id);
            questionDtos.Add(new QuestionResponseDto(
                q.Id,
                q.Text,
                q.Image,
                options.Select(o => new OptionResponseDto(o.Id, o.OptionText)).ToList()
            ));
        }

        return new QuizResponseDto(quiz.Id, quiz.Title, quiz.Description, quiz.Image, questionDtos);
    }

    public async Task<QuizResponseDto> CreateAsync(CreateQuizDto dto)
    {
        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            Image = dto.Image
        };

        await _unitOfWork.Quizzes.AddAsync(quiz);
        await _unitOfWork.CompleteAsync();

        if (dto.Questions != null && dto.Questions.Any())
        {
            foreach (var qDto in dto.Questions)
            {
                var question = new Question
                {
                    QuizId = quiz.Id,
                    Text = qDto.Text,
                    Image = qDto.Image
                };
                await _unitOfWork.Questions.AddAsync(question);
                await _unitOfWork.CompleteAsync();

                if (qDto.Options != null)
                {
                    foreach (var oDto in qDto.Options)
                    {
                        var option = new QuestionOption
                        {
                            QuestionId = question.Id,
                            OptionText = oDto.OptionText,
                            IsCorrect = oDto.IsCorrect
                        };
                        await _unitOfWork.Options.AddAsync(option);
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        return await GetByIdAsync(quiz.Id);
    }

    public async Task<QuizResponseDto> UpdateAsync(int id, UpdateQuizDto dto)
    {
        var quiz = (await _unitOfWork.Quizzes.FindWithIncludesAsync(e=>e.Id==id,e=>e.Questions, e=>e.Questions.Select(q=>q.Options))).FirstOrDefault();
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with id {id} not found.");

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.Image = dto.Image;

        foreach (var qId in dto.QuestionsToDelete)
        {
            var questionToDelete = quiz.Questions.FirstOrDefault(q => q.Id == qId);
            if (questionToDelete != null)
            {
                _unitOfWork.Questions.Remove(questionToDelete);
            }
        }

        foreach (var qDto in dto.Questions)
        {
            if (qDto.Id == 0)
            {
                var newQuestion = new Question
                {
                    Text = qDto.Text,
                    Image = qDto.Image,
                    QuizId = id,
                    Options = new List<QuestionOption>()
                };

                foreach (var oDto in qDto.Options)
                {

                    newQuestion.Options.Add(new QuestionOption
                    {
                        OptionText = oDto.OptionText,
                        IsCorrect = oDto.IsCorrect
                    });
                }

                quiz.Questions.Add(newQuestion);
            }
            else
            {
                var existingQuestion = quiz.Questions.FirstOrDefault(q => q.Id == qDto.Id);

                if (existingQuestion != null)
                {
                    existingQuestion.Text = qDto.Text;
                    existingQuestion.Image = qDto.Image;

                    foreach (var oId in qDto.OptionsToDelete)
                    {
                        var optionToDelete = existingQuestion.Options.FirstOrDefault(o => o.Id == oId);
                        if (optionToDelete != null)
                        {
                            _unitOfWork.Options.Remove(optionToDelete);
                        }
                    }

 
                    foreach (var oDto in qDto.Options)
                    {
                        if (oDto.Id == 0)
                        {
                            existingQuestion.Options.Add(new QuestionOption
                            {
                                OptionText = oDto.OptionText,
                                IsCorrect = oDto.IsCorrect
                            });
                        }
                        else
                        {
                            var existingOption = existingQuestion.Options.FirstOrDefault(o => o.Id == oDto.Id);
                            if (existingOption != null)
                            {
                                existingOption.OptionText = oDto.OptionText;
                                existingOption.IsCorrect = oDto.IsCorrect;
                            }
                        }
                    }
                }
            }
        }

        _unitOfWork.Quizzes.Update(quiz);
        await _unitOfWork.CompleteAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id);
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with id {id} not found.");

        _unitOfWork.Quizzes.Remove(quiz);
        await _unitOfWork.CompleteAsync();
    }
}
