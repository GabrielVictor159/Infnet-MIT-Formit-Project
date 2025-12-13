using Formit.Application.Interfaces;
using Formit.Domain.Entities;
using Formit.Infraestructure.Interfaces;
using Formit.Shared.DTOs;

namespace Formit.Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmissionService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<int> SubmitQuizAsync(SubmitFormDto dto)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dto.QuizId);
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with id {dto.QuizId} not found.");

        var submission = new FormSubmission
        {
            QuizId = dto.QuizId,
            UserName = _currentUserService.FullName ?? _currentUserService.Email,
            SubmissionDate = DateTime.UtcNow,
            Score = 0
        };

        var responsesToSave = new List<QuestionResponse>();

        foreach (var answerDto in dto.Answers)
        {
            var chosenOption = await _unitOfWork.Options.GetByIdAsync(answerDto.ChosenOptionId);

            if (chosenOption == null)
                throw new ArgumentException($"Option ID {answerDto.ChosenOptionId} not found.");

            if (chosenOption.QuestionId != answerDto.QuestionId)
                throw new ArgumentException($"Option ID {answerDto.ChosenOptionId} does not belong to Question ID {answerDto.QuestionId}.");

            if (chosenOption.IsCorrect)
            {
                submission.Score += 1;
            }
            responsesToSave.Add(new QuestionResponse
            {
                QuestionId = answerDto.QuestionId,
                ChosenOptionId = answerDto.ChosenOptionId
            });
        }

        await _unitOfWork.Submissions.AddAsync(submission);
        await _unitOfWork.CompleteAsync();

        foreach (var response in responsesToSave)
        {
            response.FormSubmissionId = submission.Id;
            await _unitOfWork.Responses.AddAsync(response);
        }

        await _unitOfWork.CompleteAsync();

        return submission.Id;
    }

    public async Task<IEnumerable<SubmissionSummaryDto>> GetSubmissionsByQuizIdAsync(int quizId)
    {
        var submissions = await _unitOfWork.Submissions.FindAsync(s => s.QuizId == quizId);

        return submissions.Select(s => new SubmissionSummaryDto(
            s.Id,
            s.UserName,
            s.SubmissionDate,
            s.Score
        ));
    }

    public async Task<SubmissionDetailsDto> GetSubmissionDetailsAsync(int submissionId)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(submissionId);
        if (submission == null)
            throw new KeyNotFoundException($"Submission with id {submissionId} not found.");

        var responses = await _unitOfWork.Responses.FindAsync(r => r.FormSubmissionId == submissionId);
        var answerDetails = new List<AnswerDetailDto>();

        foreach (var r in responses)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(r.QuestionId);
            var chosenOption = await _unitOfWork.Options.GetByIdAsync(r.ChosenOptionId);

            answerDetails.Add(new AnswerDetailDto(
                question!.Text,
                chosenOption!.OptionText,
                chosenOption.IsCorrect
            ));
        }

        return new SubmissionDetailsDto(
            submission.Id,
            submission.UserName,
            submission.SubmissionDate,
            submission.Score,
            answerDetails
        );
    }
}