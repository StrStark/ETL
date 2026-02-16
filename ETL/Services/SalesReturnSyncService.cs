using Dapper;
using ETL.Entity;
using ETL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ETL.Services;

public partial class SalesReturnSyncService : IEntitySyncService
{
    private readonly string _etlConn;
    private readonly ApplicationDbContext.ApplicationDbContext _db;

    public string EntityName => "salesreturn";
    public SalesReturnSyncService(string etlConn, ApplicationDbContext.ApplicationDbContext db)
    {
        _etlConn = etlConn ?? throw new ArgumentNullException(nameof(etlConn));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    private string SalesQuery => @"
        SELECT 
            s.ID as SandID,
            s.SanadNo as DocNum,
            s.HesabID as AccountCode,
            s.ArticleDesc as Article,
            s.FactorNum as HavaleCode,
            k.PRGCode as Type,
            s.OperationDate as Date,
            h.KCode as KalaCode,
            k.KCodeDelimiter KalDelimiter,
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
                    select
                    rhi.* , rh.RHCodeSenderReciver , RHSanadDate
                        from Anbar.dbo.ResidHavale as rh 
                        full outer join  Anbar.dbo.ResidHavaleItems as rhi 
                            on rh.RHCode = rhi.RHCode and  rh.RHAction = rhi.RHAction and rh.ACode = rhi.ACode and rh.RHYearID = rhi.RHIYearID
                   where rh.RHTCode IN (153, 151,146, 106, 96, 57, 32 , 33) --- wich one is sales actually ? 
                    and (rh.RHAction = 1) 
                    
                    ) AS h
            ON h.RHCode = s.FactorNum and h.ACode = s.AnbarCode and s.OperationDate = h.RHSanadDate
        LEFT JOIN Anbar.dbo.Kala as k on h.KCode = k.KCode
        left join Anbar.dbo.Person as p on h.RHCodeSenderReciver = p.PGCodeComputed
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
        
        WHERE s.HesabID IN (
            '6020000001000000100000000000000',
            '6020000001000000200000000000000',
            '6020000001000000300000000000000',
            '6020000001000000400000000000000',
            '6020000002000000100000000000000',
            '6020000002000000200000000000000',
            '6020000002000000300000000000000')
            and FactorNum is not null 
        order by OperationDate;

    ";
    public async Task<IReadOnlyList<ISyncEntity>> LoadCurrentAsync(CancellationToken ct)
    {
        using var conn = new SqlConnection(_etlConn);
        await conn.OpenAsync(ct);

        // Query the ETL database
        var sales = (await conn.QueryAsync<SaleReturnRecordModel>(SalesQuery)).ToList();

        // Assign SyncKey dynamically (if not already assigned)
        foreach (var item in sales)
        {
            item.SyncKey = GetSyncKey(item);
        }
        AssignRowIdentities(sales);
        return sales.Cast<ISyncEntity>().ToList();
    }

    public async Task<IReadOnlyList<ISyncEntity>> LoadSnapshotAsync(CancellationToken ct)
    {
        var snapshot = await _db.SaleReturnRecords
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
        _db.SaleReturnRecords.RemoveRange(_db.SaleReturnRecords);
        await _db.SaleReturnRecords.AddRangeAsync(snapshot.Cast<SaleReturnRecordModel>(), ct);
        await _db.SaveChangesAsync(ct);
    }
    public async Task ClearSnapshot(CancellationToken ct)
    {
        await _db.SaleReturnRecords.ExecuteDeleteAsync(ct);
    }
    private string GetSyncKey(SaleReturnRecordModel value)
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
    private static void AssignRowIdentities(List<SaleReturnRecordModel> items)
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
