using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagementApi.Models;

public class Assignment
{
    [Key]
    public string Id { get; set; }

    [Required(ErrorMessage = "TraineeId is required.")]
    public string TraineeId { get; set; }

    public Trainee Trainee { get; set; }

    [Required(ErrorMessage = "MentorId is required.")]
    public string MentorId { get; set; }

    public Mentor Mentor { get; set; }

    [Required(ErrorMessage = "LearningTaskId is required.")]
    public string LearningTaskId { get; set; } 
    public LearningTask LearningTask { get; set; } 


    [Required(ErrorMessage = "AssignedDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly AssignedDate { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; }

    public string Remarks { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }
}