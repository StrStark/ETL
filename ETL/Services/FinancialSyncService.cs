using Dapper;
using ETL.Entity;
using ETL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ETL.Services;

public partial class FinancialSyncService : IEntitySyncService
{
    private readonly string _etlConn;
    private readonly ApplicationDbContext.ApplicationDbContext _db;

    public string EntityName => "financial";
    public FinancialSyncService(string etlConn, ApplicationDbContext.ApplicationDbContext db)
    {
        _etlConn = etlConn ?? throw new ArgumentNullException(nameof(etlConn));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    private string SalesQuery => @"
       select 
        	Hesab.HesabID as Id,
        	info.KolCode as Code,
        	Hesab.Name as Name,
        	gr.GroupName as Type,
        	info.bedehkar,
        	info.bestankar
        from
        	(SELECT KolCode,
        		sum(BedehkarSum) as bedehkar ,
        		sum(BestankarSum) as bestankar
        	FROM [Hesab].[dbo].[Hesabs]  as Hesab
        		left join [Hesab].[dbo].[SanadData] as Sanad on Hesab.HesabID = Sanad.HesabID
        	group by KolCode) 
        as Info
        join [Hesab].[dbo].[Hesabs] as hesab on info.KolCode = hesab.KolCode
        join [Hesab].[dbo].[HesabGroups] as gr on gr.GroupID = hesab.GroupID
        where MoeenCode = '0000000'

    ";
    public async Task<IReadOnlyList<ISyncEntity>> LoadCurrentAsync(CancellationToken ct)
    {
        using var conn = new SqlConnection(_etlConn);
        await conn.OpenAsync(ct);

        // Query the ETL database
        var finantialRecords = (await conn.QueryAsync<HesabSummaryModel>(SalesQuery)).ToList();

        // Assign SyncKey dynamically (if not already assigned)
        foreach (var item in finantialRecords)
        {
            item.SyncKey = GetSyncKey(item);
        }
        AssignRowIdentities(finantialRecords);

        return finantialRecords.Cast<ISyncEntity>().ToList();
    }

    public async Task<IReadOnlyList<ISyncEntity>> LoadSnapshotAsync(CancellationToken ct)
    {
        var snapshot = await _db.HesabRecords
           .AsNoTracking()
           .ToListAsync(ct);

        // Ensure SyncKey is set
        foreach (var item in snapshot)
        {
            item.SyncKey = GetSyncKey(item);
        }
        AssignRowIdentities(snapshot);

        return snapshot.Cast<ISyncEntity>().ToList();
    }

    public async Task SaveSnapshotAsync(IEnumerable<ISyncEntity> snapshot, CancellationToken ct)
    {
        _db.HesabRecords.RemoveRange(_db.HesabRecords);
        await _db.HesabRecords.AddRangeAsync(snapshot.Cast<HesabSummaryModel>(), ct);
        await _db.SaveChangesAsync(ct);
    }
    private string GetSyncKey(HesabSummaryModel value)
    {
        if (value == null)
            return string.Empty;

        string Normalize(string? s)
            => (s ?? string.Empty).Trim().ToUpperInvariant();

        var raw = string.Join("|", new[]
        {
            Normalize(value.Bedehkar),
            Normalize(value.Bestankar),
            Normalize(value.Name),
            Normalize(value.Code),
            Normalize(value.Type),
        });

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash); // .NET 5+
    }
    private static void AssignRowIdentities(List<HesabSummaryModel> items)
    {
        // Group by SyncKey (same content)
        var groups = items.GroupBy(x => x.SyncKey);

        foreach (var group in groups)
        {
            int index = 1;
            // deterministic ordering within the group (optional, for stability)
            foreach (var item in group.OrderBy(x => x.Name)
                                      .ThenBy(x => x.Code)
                                      .ThenBy(x => x.Type))
            {
                // RowIdentity = hash + group index
                item.RowIdentity = $"{group.Key}#{index}";
                index++;
            }
        }
    }

}
