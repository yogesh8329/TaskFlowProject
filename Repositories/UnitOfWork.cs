using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<TaskItem> Tasks { get; }
        public IGenericRepository<User> Users { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Tasks = new GenericRepository<TaskItem>(_context);
            Users = new GenericRepository<User>(_context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
