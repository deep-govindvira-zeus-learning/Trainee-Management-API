namespace TraineeManagementApi.DTOs;

public class LearningTaskResponse
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string ExpectedTechStack { get; set; }

    public DateOnly DueDate { get; set; }

    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}