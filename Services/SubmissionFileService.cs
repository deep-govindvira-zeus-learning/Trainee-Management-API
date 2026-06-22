using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Data;
using TraineeManagementApi.Services;

public class SubmissionFileService : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionFileService> _logger;

    private readonly IFileStorageService _fileStorageService;

    public SubmissionFileService(AppDbContext context, ILogger<SubmissionFileService> logger, IFileStorageService fileStorageService, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }
    public async Task<(Stream Stream, string ContentType, string FileName)>  DownloadFileAsync(string id)
    {
        var submissionFile = await _context.SubmissionFiles.FindAsync(id);
        if (submissionFile == null) throw new Exception($"SubmissionFile with id: {id} not found in db.");

        if (!await _fileStorageService.ExistsAsync(submissionFile.StorageName))
        {
            _logger.LogWarning("Metadata exists for file {Id} but physical asset is missing from storage.", id);
            throw new Exception("The file resource could not be found on disk.");
        }

        var stream = await _fileStorageService.OpenReadAsync(submissionFile.StorageName);
        return (stream, submissionFile.ContentType, submissionFile.OriginalFileName);
    }

    public async Task DeleteFileAsync(string id)
    {
        var submissionFile = await _context.SubmissionFiles.FindAsync(id);
        if (submissionFile == null) throw new Exception($"SubmissionFile with id: {id} not found in db.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.SubmissionFiles.Remove(submissionFile);
            await _context.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(submissionFile.StorageName);
            await transaction.CommitAsync();

            _logger.LogInformation("Successfully purged submissionFile record and storage node for item: {Id}", id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction execution failure while rolling back file tracking record for submissionFile reference {Id}", id);
            throw new Exception("An error occurred during transaction execution.");
        }
    }
}