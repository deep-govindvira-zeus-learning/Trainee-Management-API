namespace Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string storageName, CancellationToken cancellationToken = default);
}
