using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environmentRepo;

    public Environment2DController(IEnvironment2DRepository environmentRepo)
    {
        _environmentRepo = environmentRepo;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var environments = await _environmentRepo.GetAllByUserIdAsync(GetUserId());
        return Ok(environments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var environment = await _environmentRepo.GetByIdAsync(id);
        if (environment == null || environment.UserId != GetUserId())
            return NotFound();
        return Ok(environment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Environment2D environment)
    {
        var userId = GetUserId();

        var count = await _environmentRepo.GetCountByUserIdAsync(userId);
        if (count >= 5)
            return BadRequest("Je mag maximaal 5 werelden hebben.");

        if (string.IsNullOrEmpty(environment.Name) || environment.Name.Length > 25)
            return BadRequest("Naam moet tussen 1 en 25 karakters zijn.");

        var existingEnvironments = await _environmentRepo.GetAllByUserIdAsync(userId);
        if (existingEnvironments.Any(e => e.Name.ToLower() == environment.Name.ToLower()))
            return BadRequest("Je hebt al een wereld met deze naam.");

        environment.UserId = userId;
        var id = await _environmentRepo.CreateAsync(environment);
        return CreatedAtAction(nameof(GetById), new { id }, environment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var environment = await _environmentRepo.GetByIdAsync(id);
        if (environment == null || environment.UserId != GetUserId())
            return NotFound();

        await _environmentRepo.DeleteAsync(id);
        return NoContent();
    }
}