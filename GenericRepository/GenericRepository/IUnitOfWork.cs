using Microsoft.EntityFrameworkCore;

namespace GenericRepository
{
    public interface IUnitOfWork<out TContext>
    where TContext : DbContext, new()
    {
        TContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
