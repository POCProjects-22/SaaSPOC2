using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GenericRepository.IdentityRepository
{
    public interface IIdentityUnitOfWork<out TContext>
    where TContext : IdentityDbContext, new()
    {
        TContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
