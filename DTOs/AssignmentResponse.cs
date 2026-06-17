namespace TraineeManagementApi.DTOs;

public class AssignmentResponse
{
    public string Id { get; set; } = string.Empty;
    
    public string TraineeId { get; set; } = string.Empty;

    public string MentorId { get; set; } = string.Empty;

    public string LearningTaskId { get; set; } = string.Empty;

    public DateOnly AssignedDate { get; set; }

    public DateOnly DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}