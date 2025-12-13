using Formit.Shared.DTOs;

namespace Formit.Application.Interfaces;
public interface ISubmissionService
{
    Task<int> SubmitQuizAsync(SubmitFormDto dto);
    Task<IEnumerable<SubmissionSummaryDto>> GetSubmissionsByQuizIdAsync(int quizId);
    Task<SubmissionDetailsDto> GetSubmissionDetailsAsync(int submissionId);
}
