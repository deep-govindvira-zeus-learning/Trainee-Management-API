using Core.Interfaces;

namespace Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storageRoot;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _storageRoot = configuration["FileStorage:RootPath"]
            ?? throw new ArgumentNullException("FileStorage:RootPath configuration is missing.");

        if (!Directory.Exists(_storageRoot))
        {
            Directory.CreateDirectory(_storageRoot);
        }
    }

    public async Task<string> SaveAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default)
    {
        // Server-side storage name generation prevents naming collisions and execution attacks
        string storageName = $"{Guid.NewGuid():N}{extension.ToLowerScope()}";
        string fullPath = Path.Combine(_storageRoot, storageName);

        // Path traversal defense verification
        if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(_storageRoot), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Path traversal attempt detected.");
        }

        using var targetStream = File.Create(fullPath);
        await fileStream.CopyToAsync(targetStream, cancellationToken);
        return storageName;
    }

    public Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken = default)
    {
        string fullPath = Path.Combine(_storageRoot, storageName);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Physical file not found.", storageName);
        }
        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken = default)
    {
        string fullPath = Path.Combine(_storageRoot, storageName);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<bool> DeleteAsync(string storageName, CancellationToken cancellationToken = default)
    {
        string fullPath = Path.Combine(_storageRoot, storageName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

public static class StringExtensions
{
    public static string ToLowerScope(this string ext) => ext.StartsWith(".") ? ext.ToLower() : "." + ext.ToLower();
}
