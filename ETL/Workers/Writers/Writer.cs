using ETL.Entity;
using ETL.Models;

namespace ETL.Workers.Writers;

public class SaleRecordWriter : ISyncEntityWriter
{
    public bool CanHandle(Type entityType)
        => entityType == typeof(SaleRecordModel);

    public void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity)
        => db.SaleRecords.Add((SaleRecordModel)entity);
    public void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity)
      => db.SaleRecords.AddRange(entity.Select(x => (SaleRecordModel)x));
}

public class SaleReturnRecordWriter : ISyncEntityWriter
{
    public bool CanHandle(Type entityType)
        => entityType == typeof(SaleReturnRecordModel);

    public void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity)
        => db.SaleReturnRecords.Add((SaleReturnRecordModel)entity);
    public void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity)
      => db.SaleReturnRecords.AddRange(entity.Select(x => (SaleReturnRecordModel)x));
}
public class CounterPartyWriter : ISyncEntityWriter
{
    public bool CanHandle(Type entityType)
        => entityType == typeof(CounterPartyModel);

    public void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity)
        => db.CounterParties.Add((CounterPartyModel)entity);
    public void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity)
       => db.CounterParties.AddRange(entity.Select(x => (CounterPartyModel)x));
}

public class FinancialWriter : ISyncEntityWriter
{
    public bool CanHandle(Type entityType)
        => entityType == typeof(HesabSummaryModel);

    public void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity)
        => db.HesabRecords.Add((HesabSummaryModel)entity);

    public void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity)
       => db.HesabRecords.AddRange(entity.Select(x => (HesabSummaryModel)x));
}
public class ProductWriter : ISyncEntityWriter
{
    public bool CanHandle(Type entityType)
        => entityType == typeof(AnbarResidHavaleDetailModel);
    public void Add(ApplicationDbContext.ApplicationDbContext db, ISyncEntity entity)
           => db.ProductRecords.Add((AnbarResidHavaleDetailModel)entity);
    public void Add(ApplicationDbContext.ApplicationDbContext db, List<ISyncEntity> entity)
        => db.ProductRecords.AddRange(entity.Select(x=> (AnbarResidHavaleDetailModel)x));
}

