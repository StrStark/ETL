using Dapper;
using ETL.Entity;
using ETL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ETL.Services;

public partial class ProductSyncService : IEntitySyncService
{
    private readonly string _etlConn;
    private readonly ApplicationDbContext.ApplicationDbContext _db;

    public string EntityName => "product";
    public ProductSyncService(string etlConn, ApplicationDbContext.ApplicationDbContext db)
    {
        _etlConn = etlConn ?? throw new ArgumentNullException(nameof(etlConn));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    private string SalesQuery => @"
        select
        CASE rh.RHAction
                WHEN 1 THEN N'Receipt'
                WHEN 2 THEN N'Issue'
                WHEN 3 THEN N'SalesOrConsumptionRequest'
                when 4 then N'PurchaseOrProductionRequest'
                WHEN 5 THEN N'ProformaInvoice'
                WHEN 6 THEN N'SalesInvoice'
            END AS RHActionDesc,
            pr.PRGDesc AS Type,
            sg.SBGDesc AS DetailedType,
            REPLACE(A.ADesc, N'*', N'') AS Anbar,
            k.KCode as KalaCode,
            k.KDesc,
                rh.RegistrationDate as Date,
            rhi.RHCode as FactorNum,
            rhi.RHIUValue1 as UnitValue1,
            rhi.RHIUValue1 as UnitValue2,
            rhi.RHIUValue1 as UnitValue3,
            rhi.RHIFi as Fee,
            rhi.RHIPrice as Price,
            rh.RHDesc
        from [Anbar].[dbo].[Kala] as k
        join [Anbar].[dbo].[ResidHavaleItems] as rhi on   K.KCode = rhi.KCode
        join [Anbar].[dbo].[ResidHavale] as rh on  rh.RHCode = rhi.RHCode AND rh.RHAction = rhi.RHAction AND rh.ACode = rhi.ACode AND rh.RHYearID = rhi.RHIYearID 
        join  [Anbar].[dbo].[ResidHavaleType] as rht on rht.RHTCodeType = rh.RHTCodeType and rht.RHTCode = rh.RHTCode
        join [Anbar].[dbo].[Anbars] as A on rhi.ACode = A.ACode
        join [Anbar].[dbo].[PrimaryGroup] as pr on pr.PRGCode = rhi.PRGCode
        CROSS APPLY (
            SELECT 
                CAST(LEFT(value, CHARINDEX('-', value) - 1) AS INT) AS CreationType
            FROM STRING_SPLIT(k.KCodeDelimiter, ';')
            WHERE RIGHT(value, LEN(value) - CHARINDEX('-', value)) =
                  CASE 
                      WHEN k.PRGCode = 1 THEN '8'
                      WHEN k.PRGCode IN (2, 6) THEN '1'
                  END
        ) AS ct
        JOIN Anbar.dbo.SubGroup AS sg
            ON sg.PRGCode = k.PRGCode
            AND sg.SBGCode = ct.CreationType
            AND sg.SGCode =
            CASE 
                WHEN k.PRGCode = 1 THEN 8
                WHEN k.PRGCode IN (2, 6) THEN 1
            END
        where pr.PRGCode in (1,2,4,6)
        order by k.KCode
    ";
    public async Task<IReadOnlyList<ISyncEntity>> LoadCurrentAsync(CancellationToken ct)
    {
        using var conn = new SqlConnection(_etlConn);
        await conn.OpenAsync(ct);

        // Query the ETL database
        var sales = (await conn.QueryAsync<AnbarResidHavaleDetailModel>(SalesQuery)).ToList();

        foreach (var item in sales)
        {
            item.SyncKey = GetSyncKey(item);
        }
        AssignRowIdentities(sales);
        return sales.Cast<ISyncEntity>().ToList();
    }

    public async Task<IReadOnlyList<ISyncEntity>> LoadSnapshotAsync(CancellationToken ct)
    {
        var snapshot = await _db.ProductRecords
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
        _db.ProductRecords.RemoveRange(_db.ProductRecords);
        await _db.ProductRecords.AddRangeAsync(snapshot.Cast<AnbarResidHavaleDetailModel>(), ct);
        await _db.SaveChangesAsync(ct);
    }
    private string GetSyncKey(AnbarResidHavaleDetailModel value)
    {
        if (value == null)
            return string.Empty;

        string Normalize(string? s)
            => (s ?? string.Empty).Trim().ToUpperInvariant();

        var raw = string.Join("|", new[]
        {
            Normalize(value.RHActionDesc),
            Normalize(value.Type),
            Normalize(value.DetailedType),
            Normalize(value.Anbar),
            Normalize(value.KalaCode),
            Normalize(value.KDesc),
            Normalize(value.FactorNum),
            Normalize(value.UnitValue1),
            Normalize(value.UnitValue2),
            Normalize(value.UnitValue3),
            Normalize(value.Fee),
            Normalize(value.Price),
            Normalize(value.RHDesc),
            Normalize(value.Date),
        });

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash); // .NET 5+
    }

    private static void AssignRowIdentities(List<AnbarResidHavaleDetailModel> items)
    {
        // Group by SyncKey (same content)
        var groups = items.GroupBy(x => x.SyncKey);

        foreach (var group in groups)
        {
            int index = 1;
            // deterministic ordering within the group (optional, for stability)
            foreach (var item in group.OrderBy(x => x.Anbar)
                                      .ThenBy(x => x.KalaCode)
                                      .ThenBy(x => x.DetailedType))
            {
                // RowIdentity = hash + group index
                item.RowIdentity = $"{group.Key}#{index}";
                index++;
            }
        }
    }

}
