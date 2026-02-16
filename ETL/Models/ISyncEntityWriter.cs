namespace ETL.Models;

public interface ISyncEntityWriter
{
    bool CanHandle(Type entityType);
    void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity);
    void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity);

}
