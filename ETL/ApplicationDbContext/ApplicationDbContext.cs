using ETL.Entity;
using Microsoft.EntityFrameworkCore;

namespace ETL.ApplicationDbContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SaleRecordModel> SaleRecords { get; set; }
    public DbSet<CounterPartyModel> CounterParties { get; set; }
    public DbSet<SaleReturnRecordModel> SaleReturnRecords { get; set; }
    public DbSet<HesabSummaryModel> HesabRecords { get; set; }
    public DbSet<AnbarResidHavaleDetailModel> ProductRecords { get; set; }
}
