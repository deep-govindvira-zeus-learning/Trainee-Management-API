namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/mentors")]
[Authorize]
public class MentorController : ControllerBase
{
    private readonly IMentorService _service;

    public MentorController(IMentorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMentor()
    {
        var responses = await _service.GetAllMentorAsync();
        return Ok(responses);
    }
}
