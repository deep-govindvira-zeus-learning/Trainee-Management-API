using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagementApi.Models;

public class Trainee
{
    [Key]
    public string Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters are allowed in First name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage ="Maximum 50 characters are allowed in Last name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Valid email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "TechStack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }
}

