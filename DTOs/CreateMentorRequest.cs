namespace TraineeManagementApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateMentorRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters are allowed in First name.")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage ="Maximum 50 characters are allowed in Last name.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Valid email is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expertise is required.")]
    public string Expertise { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [AllowedValues("Active", "Inactive", ErrorMessage = "Status must be Active or Inactive.")]
    public string Status { get; set; } = string.Empty;
}
