namespace RealProject.Models
{
    public interface IRepository<TEntity>
    {
        void Add(TEntity entity);
        void Delete(int id);
        void Update(TEntity entity);
        List<TEntity> GetAll();
        TEntity GetById(int id);
    }
}
