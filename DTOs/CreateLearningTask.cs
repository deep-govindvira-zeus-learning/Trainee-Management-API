using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CreateLearningTask
{

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string ExpectedTechStack { get; set; }

    [Required]
    public DateOnly DueDate { get; set; }

    [Required]
    public string Status { get; set; }
}