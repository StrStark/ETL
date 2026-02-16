namespace ETL.Models;

public interface ISyncEntity
{
    string SyncKey { get; set; }
    string RowIdentity { get; set; }
}
