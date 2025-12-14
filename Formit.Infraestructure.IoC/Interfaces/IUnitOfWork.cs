using Formit.Domain.Entities;

namespace Formit.Infraestructure.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Quiz> Quizzes { get; }
    IGenericRepository<Question> Questions { get; }
    IGenericRepository<QuestionOption> Options { get; }
    IGenericRepository<FormSubmission> Submissions { get; }
    IGenericRepository<QuestionResponse> Responses { get; }
    Task<int> CompleteAsync();
}
