using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<TaskItem> Tasks { get; }
        IGenericRepository<User> Users { get; }

        Task<int> SaveChangesAsync();
    }
}
