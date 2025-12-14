using Formit.Domain.Entities;
using Formit.Infraestructure.Data.Contexts;
using Formit.Infraestructure.Interfaces;

namespace Formit.Infraestructure.Data.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IGenericRepository<Quiz> Quizzes { get; private set; }
    public IGenericRepository<Question> Questions { get; private set; }
    public IGenericRepository<QuestionOption> Options { get; private set; }
    public IGenericRepository<FormSubmission> Submissions { get; private set; }
    public IGenericRepository<QuestionResponse> Responses { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Quizzes = new GenericRepository<Quiz>(_context);
        Questions = new GenericRepository<Question>(_context);
        Options = new GenericRepository<QuestionOption>(_context);
        Submissions = new GenericRepository<FormSubmission>(_context);
        Responses = new GenericRepository<QuestionResponse>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
