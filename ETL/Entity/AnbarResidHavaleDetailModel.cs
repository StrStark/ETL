using ETL.Models;
using ETL.Services;
using System.ComponentModel.DataAnnotations;

namespace ETL.Entity;

public class AnbarResidHavaleDetailModel : ISyncEntity
{
    [Key]
    public Guid ID { get; set; }
    public string? RHActionDesc { get; set; }
    public string? Type { get; set; }        
    public string? DetailedType { get; set; }
    public string? Anbar { get; set; }       
    public string? KalaCode { get; set; }    
    public string? KDesc { get; set; }       
    public string? FactorNum { get; set; }   
    public string? UnitValue1 { get; set; }  
    public string? UnitValue2 { get; set; }  
    public string? UnitValue3 { get; set; }  
    public string? Fee { get; set; }         
    public string? Price { get; set; }       
    public string? RHDesc { get; set; }      
    public string? Date { get; set; }

    public string RowIdentity { get; set; } = default!;
    public string SyncKey { get; set; } = default!;
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
}
