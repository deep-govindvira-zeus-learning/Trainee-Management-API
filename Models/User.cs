using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagementApi.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [RegularExpression("^(Admin|Mentor|Trainee)$", ErrorMessage = "Role must be Admin, Mentor, or Trainee.")]
    public string Role { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

}

