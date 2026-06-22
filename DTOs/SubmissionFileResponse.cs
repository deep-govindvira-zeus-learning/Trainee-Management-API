namespace TraineeManagementApi.DTOs;


public class SubmissionFileResponse
{
    public string Id { get; set; } = string.Empty;
    public string SubmissionId { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StorageName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}