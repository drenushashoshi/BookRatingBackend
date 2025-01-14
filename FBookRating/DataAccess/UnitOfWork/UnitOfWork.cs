using FBookRating.DataAccess.Context;
using FBookRating.DataAccess.Repository;
using System.Collections;

namespace FBookRating.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Complete()
        {
            var numberOfAffectedRows = _dbContext.SaveChanges();
            return numberOfAffectedRows > 0;
        }

        public IBookRatingRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.Contains(type))
            {
                var repositoryType = typeof(BookRatingRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IBookRatingRepository<TEntity>)_repositories[type];
        }
    }
}
