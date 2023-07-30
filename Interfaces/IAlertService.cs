namespace Realert.Interfaces
{
    public interface IAlertService<T>
    {
        Task AddAlertAsync(T alert);
        Task DeleteAlertAsync(T alert, bool isDelist);
        Task PerformScanAsync();
    }
}
