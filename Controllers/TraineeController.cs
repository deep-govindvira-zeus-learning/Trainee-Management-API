namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/trainees")]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;

    public TraineesController(ITraineeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetALlTrainees()
    {
        return Ok(_service.GetAllTrainees());
    }

    [HttpGet("{id}")]
    public IActionResult GetTraineeById(string id)
    {
        var response = _service.GetTraineeResponseById(id);

        if (response == null) return NotFound($"Trainee with ID {id} was not found.");

        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateTrainee([FromBody] CreateTraineeRequest request)
    {
        var response = _service.CreateTrainee(request);

        if (response == null) return BadRequest($"Trainee is not created.");

        return CreatedAtAction(nameof(GetTraineeById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTrainee(string id, [FromBody] UpdateTraineeRequest request)
    {
        if (request == null) return BadRequest();

        var response = _service.UpdateTrainee(id, request);

        if (response == null) return NotFound($"Trainee with ID {id} was not found.");

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTrainee(string id)
    {
        if (!_service.DeleteTraineeById(id)) return NotFound();

        return NoContent();
    }
}
