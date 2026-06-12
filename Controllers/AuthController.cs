using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _service.Login(request);   
        return Ok(response);
    }
}
