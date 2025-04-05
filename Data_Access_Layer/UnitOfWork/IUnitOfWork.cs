using Data_Access_Layer.Repository;

namespace Data_Access_Layer.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IBookRatingRepository<TEntity> Repository<TEntity>() where TEntity : class;

        bool Complete();
    }
}
