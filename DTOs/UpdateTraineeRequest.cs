namespace TraineeManagementApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class UpdateTraineeRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage ="Maximum 50 characters are allowed in First name")]
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
    [AllowedValues("Active", "Inactive", "Pending", ErrorMessage = "Status must be Active, Inactive, or Pending")]
    public string Status { get; set; }
}
