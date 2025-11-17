namespace BusinessSharkClient.Data.Entities.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime UpdatedAt { get; set; }  // UTC
        bool IsDirty { get; set; }        // need to sync with server
        bool IsDeleted { get; set; }      // soft-delete
    }
}
