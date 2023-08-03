namespace Realert.Interfaces
{
    public interface IAlertService<T>
    {
        Task<int> AddAlertAsync(T alert);

        Task DeleteAlertAsync(T alert);

        Task PerformScanAsync();
    }
}
