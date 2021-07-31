using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Data;

namespace UserService.Services
{
    public sealed class UnitOfWork
    {
        private Dictionary<string, object> Repositories { get; set; }

        private readonly UserServiceContext _context;

        public UnitOfWork(UserServiceContext context)
        {
            _context = context;
            Repositories = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public GenericRepository<T> Repository<T>() where T : class
        {
            var typeName = typeof(T).Name;
            if (Repositories.ContainsKey(typeName))
            {
                return Repositories[typeName] as GenericRepository<T>;
            }

            GenericRepository<T> repo = new GenericRepository<T>(_context);
            Repositories.Add(typeName, repo);
            return repo;
        }

        public int SaveChanges() => _context.SaveChanges();

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
