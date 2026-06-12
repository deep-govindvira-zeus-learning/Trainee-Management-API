using System.ComponentModel.DataAnnotations;

public class UpdateLearningTaskRequest
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string ExpectedTechStack { get; set; }

    [Required]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly DueDate { get; set; }

    [Required]
    public string Status { get; set; }
}