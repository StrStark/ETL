namespace ETL.SqlQueries;

public static class Queries
{
    public static string SalesQuery(List<string> TargetSalesAccounts , List<string> RhTypes , Dictionary<int , int> PrimaryGroupCategoryIdentifier) => @$"
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
        WHERE rh.RHTCode IN ({string.Join(',' , RhTypes)})
        AND (rh.RHAction IN (2))
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
              CASE {
                string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())
             }END
    ) AS ct
    LEFT JOIN Anbar.dbo.SubGroup AS sg
        ON sg.PRGCode = k.PRGCode
        AND sg.SBGCode = ct.CreationType
        AND sg.SGCode =
        CASE {
            string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())
       }END
    WHERE s.HesabID in ({string.Join(',' , TargetSalesAccounts.Select(s=> $"'{s}'"))}) AND FactorNum IS NOT NULL



    ";
    public static string SalesReturnQuery(List<string> TargetSalesReturnAccounts, List<string> RhTypes, Dictionary<int, int> PrimaryGroupCategoryIdentifier) => @$"
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
        WHERE rh.RHTCode IN ({string.Join(',', RhTypes)})
        AND (rh.RHAction IN (2))
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
              CASE {string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())}END
    ) AS ct
    LEFT JOIN Anbar.dbo.SubGroup AS sg
        ON sg.PRGCode = k.PRGCode
        AND sg.SBGCode = ct.CreationType
        AND sg.SGCode =
        CASE {string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())}END
    WHERE s.HesabID in ({string.Join(',', TargetSalesReturnAccounts.Select(s => $"'{s}'"))}) AND FactorNum IS NOT NULL



    ";
    public static string CounterPartyQuery(string TargetLetter) => $@"
    
    SELECT
        h.HesabID,
        h.TafsiliCode as CounterPartyCode,
        h.Name as FullName , 
        p.PHesabID as HesabAnbar,
        p.PGCode as CustomerType,
        p.PName as CustomerName,
        p.PFamily as CusotmerFamily,
        p.PGCodeComputed as CustomerCode,
        p.PCodeMeliOrSH_Sabt as CustomerIdentiifre,
        p.PTel as CustomerTel,
        p.PMobile as CustomerPhoneNumber,
        p.PEmail as CustomerPelak,
        p.PAddress as CustomerAddress,
        p.PBirthday as CustomerBirthDay,
        p.PEnable as Active
    FROM [Hesab].[dbo].[Hesabs] h
    RIGHT JOIN [Anbar].[dbo].[Person] p
        ON p.PHesabID LIKE '%' + h.TafsiliCode + '%'
       AND h.StructureID = 5
       AND h.TafsiliCode LIKE 'D%'
    WHERE
        p.PHesabID LIKE '%{TargetLetter}%'
    
    ";
    public static string ProductsQuery(Dictionary<int, int> PrimaryGroupCategoryIdentifier) => $@"
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
             CASE {string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())}END
    ) AS ct
    JOIN Anbar.dbo.SubGroup AS sg
        ON sg.PRGCode = k.PRGCode
        AND sg.SBGCode = ct.CreationType
        AND sg.SGCode =
        CASE {string.Join("\n", PrimaryGroupCategoryIdentifier.Select(s => $"WHEN k.PRGCode = {s.Key} THEN '{s.Value}'").ToList())}END
    where pr.PRGCode in ({string.Join("," , PrimaryGroupCategoryIdentifier.Select(s=>$"{s.Key}").ToList())})
    order by k.KCode
    ";
    public static string FinancialQuery() => $@"

    SELECT 
        h.HesabID AS Id,
        i.KolCode AS Code,
        h.Name AS Name,
        g.GroupName AS Type,
        i.bedehkar AS Bedehkar,
        i.bestankar AS Bestankar
    FROM
        (
            SELECT 
                KolCode,
                SUM(BedehkarSum) AS bedehkar,
                SUM(BestankarSum) AS bestankar
            FROM [Hesab].[dbo].[Hesabs] AS Hesab
            LEFT JOIN [Hesab].[dbo].[SanadData] AS Sanad ON Hesab.HesabID = Sanad.HesabID
            GROUP BY KolCode
        ) AS i
    JOIN [Hesab].[dbo].[Hesabs] AS h ON i.KolCode = h.KolCode
    JOIN [Hesab].[dbo].[HesabGroups] AS g ON g.GroupID = h.GroupID
    WHERE h.MoeenCode = '0000000';
    
    ";
}
