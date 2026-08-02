namespace TraineeManagementApi.Services;

public interface ISubmissionFileService
{
    Task<(Stream Stream, string ContentType, string FileName)> DownloadFileAsync(string id, string requestedBy, bool isPrivileged);
    Task DeleteFileAsync(string id, string requestedBy, bool isPrivileged);

}
