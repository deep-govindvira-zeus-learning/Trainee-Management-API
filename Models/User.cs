using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagementApi.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public string Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "PasswordHash is required.")]
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; set; }

}

