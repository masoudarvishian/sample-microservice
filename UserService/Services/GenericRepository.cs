using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using UserService.Data;

namespace UserService.Services
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly UserServiceContext _context;

        public GenericRepository(UserServiceContext context)
        {
            _context = context;
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return _context.Set<TEntity>()
                .AsNoTracking();
        }

        public virtual void Create(TEntity entity)
        {
            _context.Set<TEntity>()
                .Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Set<TEntity>()
                .Update(entity);
        }

        public virtual IQueryable<TEntity> FindByCondition(
            Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>()
                .Where(expression)
                ?.AsNoTracking();
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }
}
