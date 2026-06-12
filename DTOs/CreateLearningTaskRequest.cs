using System.ComponentModel.DataAnnotations;

public class CreateLearningTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters are allowed in Title.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "ExpectedTechStack is required.")]
    public string ExpectedTechStack { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [AllowedValues("Draft", "Published", "Closed", ErrorMessage = "Status must be Draft, Published, or Closed.")]
    public string Status { get; set; }
}