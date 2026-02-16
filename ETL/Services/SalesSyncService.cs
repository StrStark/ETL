using Dapper;
using ETL.Entity;
using ETL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ETL.Services;

public partial class SalesSyncService : IEntitySyncService
{
    private readonly string _etlConn;
    private readonly ApplicationDbContext.ApplicationDbContext _db;

    public string EntityName => "Sales";
    public SalesSyncService(string etlConn, ApplicationDbContext.ApplicationDbContext db)
    {
        _etlConn = etlConn ?? throw new ArgumentNullException(nameof(etlConn));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    private string SalesQuery => @"
        SELECT 
            s.SanadNo as DocNum,
            s.HesabID as AccountCode,
            s.ArticleDesc as Article,
            s.FactorNum as HavaleCode,
            k.PRGCode as Type,
            s.OperationDate as Date,
            h.KCode as KalaCode,
            k.KCodeDelimiter as KalDelimiter,
            k.KDesc,
            h.RHIUCode1 as UnitCode1,
            h.RHIUValue1 as UnitValue1,
            h.RHIUCode1 as UnitCode2,
            h.RHIUValue1 as UnitValue2,
            h.RHIUCode1 as UnitCode3,
            h.RHIUValue1 as UnitValue3,
            h.RHIPrice as Price,
            h.RHCodeSenderReciver as TarafHesab,
            p.PGCode as CustomerType,
            p.PHesabID as CustomerAccount,
            p.PName as CustomerName,
            p.PFamily as CusotmerFamily,
            p.PGCodeComputed as CustomerCode,
            p.PCodeMeliOrSH_Sabt as CustomerIdentiifre,
            p.PTel as CustomerTel,
            p.PMobile as CustomerPhoneNumber,
            p.PEmail as CustomerPelak,
            p.PAddress as CustomerAddress,
            p.PBirthday as CustomerBirthDay,
            p.PEnable as Active,
            sg.SBGDesc AS DetailedType
        FROM Hesab.dbo.SanadData AS s
        LEFT JOIN (
            SELECT
                rhi.*, rh.RHCodeSenderReciver, RHSanadDate
            FROM Anbar.dbo.ResidHavale as rh 
            FULL OUTER JOIN Anbar.dbo.ResidHavaleItems as rhi 
                ON rh.RHCode = rhi.RHCode 
                AND rh.RHAction = rhi.RHAction 
                AND rh.ACode = rhi.ACode 
                AND rh.RHYearID = rhi.RHIYearID
            WHERE rh.RHTCode IN (153, 151, 145, 106, 96, 82, 81, 62, 57, 32)
            AND (rh.RHAction = 2)
        ) AS h
            ON h.RHCode = s.FactorNum 
            AND h.ACode = s.AnbarCode 
            AND s.OperationDate = h.RHSanadDate
        LEFT JOIN Anbar.dbo.Kala as k on h.KCode = k.KCode
        LEFT JOIN Anbar.dbo.Person as p on h.RHCodeSenderReciver = p.PGCodeComputed
        
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
        LEFT JOIN Anbar.dbo.SubGroup AS sg
            ON sg.PRGCode = k.PRGCode
            AND sg.SBGCode = ct.CreationType
            AND sg.SGCode =
            CASE 
                WHEN k.PRGCode = 1 THEN 8
                WHEN k.PRGCode IN (2, 6) THEN 1
            END
        WHERE s.HesabID like '6010000003%' AND FactorNum IS NOT NULL
    ";
    public async Task<IReadOnlyList<ISyncEntity>> LoadCurrentAsync(CancellationToken ct)
    {
        using var conn = new SqlConnection(_etlConn);
        await conn.OpenAsync(ct);
        var sales = (await conn.QueryAsync<SaleRecordModel>(SalesQuery)).ToList();

        foreach (var item in sales)
        {
            item.SyncKey = GetSyncKey(item);
        }
        AssignRowIdentities(sales);
        return sales.Cast<ISyncEntity>().ToList();
    }

    public async Task<IReadOnlyList<ISyncEntity>> LoadSnapshotAsync(CancellationToken ct)
    {
        var snapshot = await _db.SaleRecords
           .AsNoTracking()
           .ToListAsync(ct);

        foreach (var item in snapshot)
        {
            item.SyncKey = GetSyncKey(item);
        }

        AssignRowIdentities(snapshot);

        return snapshot.Cast<ISyncEntity>().ToList();
    }

    public async Task ClearSnapshot(CancellationToken ct)
    {
        await _db.SaleRecords.ExecuteDeleteAsync(ct);
    }
    public async Task SaveSnapshotAsync(IEnumerable<ISyncEntity> snapshot, CancellationToken ct)
    {
        _db.SaleRecords.RemoveRange(_db.SaleRecords);
        await _db.SaleRecords.AddRangeAsync(snapshot.Cast<SaleRecordModel>(), ct);
        await _db.SaveChangesAsync(ct);
    }
    private string GetSyncKey(SaleRecordModel value)
    {
        if (value == null)
            return string.Empty;

        string Normalize(string? s)
            => (s ?? string.Empty).Trim().ToUpperInvariant();

        var raw = string.Join("|", new[]
        {
            Normalize(value.SandID),
            Normalize(value.DocNum),
            Normalize(value.AccountCode),
            Normalize(value.Article),
            Normalize(value.HavaleCode),
            Normalize(value.Type),
            Normalize(value.Date),
            Normalize(value.KalaCode),
            Normalize(value.KalDelimiter),
            Normalize(value.KDesc),
            Normalize(value.UnitCode1),
            Normalize(value.UnitValue1),
            Normalize(value.UnitCode2),
            Normalize(value.UnitValue2),
            Normalize(value.UnitCode3),
            Normalize(value.UnitValue3),
            Normalize(value.Price),
            Normalize(value.TarafHesab),
            Normalize(value.CustomerType),
            Normalize(value.CustomerAccount),
            Normalize(value.CustomerName),
            Normalize(value.CusotmerFamily),
            Normalize(value.CustomerCode),
            Normalize(value.CustomerIdentiifre),
            Normalize(value.CustomerTel),
            Normalize(value.CustomerPhoneNumber),
            Normalize(value.CustomerPelak),
            Normalize(value.CustomerAddress),
            Normalize(value.CustomerBirthDay),
            Normalize(value.Active),
            Normalize(value.DetailedType)
        });

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash); // .NET 5+
    }
    private static void AssignRowIdentities(List<SaleRecordModel> items)
    {
        // Group by SyncKey (same content)
        var groups = items.GroupBy(x => x.SyncKey);

        foreach (var group in groups)
        {
            int index = 1;
            // deterministic ordering within the group (optional, for stability)
            foreach (var item in group.OrderBy(x => x.DocNum)
                                      .ThenBy(x => x.KalaCode)
                                      .ThenBy(x => x.Article))
            {
                // RowIdentity = hash + group index
                item.RowIdentity = $"{group.Key}#{index}";
                index++;
            }
        }
    }


}
