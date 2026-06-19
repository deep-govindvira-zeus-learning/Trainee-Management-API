using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagementApi.Models;

public class Mentor
{
    [Key]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "Maximum 50 characters are allowed in First name.")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage ="Maximum 50 characters are allowed in Last name.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Valid email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Expertise is required.")]
    public string Expertise { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } // Active / Inactive

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }
}
