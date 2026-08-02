using System.Security.Cryptography;
using Core.Interfaces;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class SubmissionFileConverter
{

    public static async Task<SubmissionFile> ToSubmissionFileAsync(string submissionId, IFormFile file, IFileStorageService fileStorageService, string userIdentity)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        string checksum;
        string storageName;

        using (var stream = file.OpenReadStream())
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = await sha256.ComputeHashAsync(stream);
            checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            stream.Position = 0; // Reset pointer position
            storageName = await fileStorageService.SaveAsync(stream, extension);
        }

        // Establish decoupled structural metadata reference mapping entity
        return new SubmissionFile
        {
            Id = Guid.NewGuid().ToString(),
            SubmissionId = submissionId,
            OriginalFileName = Path.GetFileName(file.FileName), // Prevent path traversal exploits
            StorageName = storageName,
            ContentType = file.ContentType,
            SizeInBytes = file.Length,
            Checksum = checksum,
            UploadedBy = userIdentity
        };
    }

    public static SubmissionFileResponse ToSubmissionFileResponse(SubmissionFile submissionFile)
    {
        return new SubmissionFileResponse
        {
            Id = submissionFile.Id,
            SubmissionId = submissionFile.SubmissionId,
            OriginalFileName = submissionFile.OriginalFileName,
            StorageName = submissionFile.StorageName,
            ContentType = submissionFile.ContentType,
            SizeInBytes = submissionFile.SizeInBytes,
            Checksum = submissionFile.Checksum,
            UploadedBy = submissionFile.UploadedBy,
            CreatedDate = submissionFile.CreatedDate,
            UpdatedDate = submissionFile.UpdatedDate,
        };
    }

}