using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public class UserResponse
{
    public string Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}