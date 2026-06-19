using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagementApi.Models;

public class SubmissionFile
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string SubmissionId { get; set; } = string.Empty;

    // Navigation property back to the submission parent node
    public Submission Submission { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StorageName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string Checksum { get; set; } = string.Empty; // SHA-256 hash
    public string UploadedBy { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }
}