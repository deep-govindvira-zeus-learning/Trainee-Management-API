using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
}
