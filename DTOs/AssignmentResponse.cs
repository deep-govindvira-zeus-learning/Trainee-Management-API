using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.DTOs;

public class AssignmentResponse
{
    public string Id { get; set; }
    
    public string TraineeId { get; set; }

    public string MentorId { get; set; }

    public string LearningTaskId { get; set; } 

    public DateOnly AssignedDate { get; set; }

    public DateOnly DueDate { get; set; }

    public string Status { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}