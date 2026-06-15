using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service)
    {
        _service = service;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var responses = await _service.GetAllAsync();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReviewRequest request)
    {
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }
}
