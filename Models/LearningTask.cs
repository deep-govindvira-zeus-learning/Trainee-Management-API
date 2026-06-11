using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LearningTask
{
    [Key]
    public string Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string ExpectedTechStack { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public string Status { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }
}