using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class UserRoleLookup
{
    [Key]
    public UserRole Id { get; set; } // Uses the enum as the primary key!

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } 
}
