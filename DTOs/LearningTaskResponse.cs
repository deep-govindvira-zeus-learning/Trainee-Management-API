namespace TraineeManagementApi.DTOs;

public class LearningTaskResponse
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ExpectedTechStack { get; set; } = string.Empty;

    public DateOnly DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}