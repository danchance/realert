namespace Realert.Interfaces
{
    public interface IAlertService<T>
    {
        Task AddAlertAsync(T alert);
        Task DeleteAlertAsync(T alert);
        Task PerformScanAsync();
    }
}
