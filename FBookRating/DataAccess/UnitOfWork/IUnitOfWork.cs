using FBookRating.DataAccess.Repository;

namespace FBookRating.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IBookRatingRepository<TEntity> Repository<TEntity>() where TEntity : class;

        bool Complete();
    }
}
