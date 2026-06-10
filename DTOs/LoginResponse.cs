namespace TraineeManagementApi.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public UserResponse User { get; set; } = new();

}