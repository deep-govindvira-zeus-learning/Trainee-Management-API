namespace TraineeManagementApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateTraineeRequest
{
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Valid email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "TechStack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [AllowedValues("Active", "Inactive", "Pending", ErrorMessage = "Status must be Active, Inactive, or Pending")]
    public string Status { get; set; }
}
