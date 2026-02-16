using DotNetEnv;
using ETL.Channels;
using ETL.Models;
using ETL.Services;
using ETL.SqlQueries;
using ETL.WebSocket;
using ETL.Workers;
using ETL.Workers.Writers;
using Microsoft.EntityFrameworkCore;

namespace ETL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var etlConn = /*builder.Configuration.GetConnectionString("ETL_DB")*/ "Data Source=DESKTOP-KVHBHO8\\STRSTARK;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Snapshot DB connection string (Postgres or other)
            
            var snapshotConn = /*builder.Configuration.GetConnectionString("Snapshot_DB")*/ "Host=localhost;Port=5432;Database=BineshDb;Username=StrStark;Password=Mr5568###;";
            builder.Services.AddControllers();

            builder.Services.AddSingleton<SyncChannels>();
            builder.Services.AddHostedService<SnapshotUpdaterWorker>();
            builder.Services.AddSingleton<WebSocket.WebSocketManager>();
            builder.Services.AddSingleton<IChangeSender, WebSocketChangeSender>();
            builder.Services.AddScoped<SyncOrchestrator>();
            builder.Services.AddDbContext<ApplicationDbContext.ApplicationDbContext>(op => op.UseNpgsql(snapshotConn));

            builder.Services.AddScoped<ISyncEntityWriter, SaleRecordWriter>();
            builder.Services.AddScoped<ISyncEntityWriter, CounterPartyWriter>();
            builder.Services.AddScoped<ISyncEntityWriter, SaleReturnRecordWriter>();
            builder.Services.AddScoped<ISyncEntityWriter, FinancialWriter>();
            builder.Services.AddScoped<ISyncEntityWriter, ProductWriter>();


            builder.Services.AddScoped<IEntitySyncService, SalesSyncService>(sp =>
            {
                var db = sp.GetRequiredService<ApplicationDbContext.ApplicationDbContext>();
                return new SalesSyncService(etlConn, db);
            });
            builder.Services.AddScoped<IEntitySyncService, FinancialSyncService>(sp =>
            {
                var db = sp.GetRequiredService<ApplicationDbContext.ApplicationDbContext>();
                return new FinancialSyncService(etlConn, db);
            });
            builder.Services.AddScoped<IEntitySyncService, ProductSyncService>(sp =>
            {
                var db = sp.GetRequiredService<ApplicationDbContext.ApplicationDbContext>();
                return new ProductSyncService(etlConn, db);
            });
            builder.Services.AddScoped<IEntitySyncService, SalesReturnSyncService>(sp =>
            {
                var db = sp.GetRequiredService<ApplicationDbContext.ApplicationDbContext>();
                return new SalesReturnSyncService(etlConn, db);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.Run();
        }
    }
}
