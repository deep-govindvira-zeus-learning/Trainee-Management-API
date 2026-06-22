namespace TraineeManagementApi.Services;

public interface ISubmissionFileService
{
    Task<(Stream Stream, string ContentType, string FileName)> DownloadFileAsync(string id);
    Task DeleteFileAsync(string id);

}
