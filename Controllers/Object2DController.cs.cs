using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Object2DController : ControllerBase
{
    private readonly IObject2DRepository _objectRepo;
    private readonly IEnvironment2DRepository _environmentRepo;

    public Object2DController(IObject2DRepository objectRepo, IEnvironment2DRepository environmentRepo)
    {
        _objectRepo = objectRepo;
        _environmentRepo = environmentRepo;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet("environment/{environmentId}")]
    public async Task<IActionResult> GetAllByEnvironment(Guid environmentId)
    {
        var environment = await _environmentRepo.GetByIdAsync(environmentId);
        if (environment == null || environment.UserId != GetUserId())
            return NotFound();

        var objects = await _objectRepo.GetAllByEnvironmentIdAsync(environmentId);
        return Ok(objects);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Object2D object2D)
    {
        var environment = await _environmentRepo.GetByIdAsync(object2D.Environment2DId);
        if (environment == null || environment.UserId != GetUserId())
            return NotFound();

        var id = await _objectRepo.CreateAsync(object2D);
        return Ok(new { Id = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Object2D object2D)
    {
        object2D.Id = id;
        await _objectRepo.UpdateAsync(object2D);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _objectRepo.DeleteAsync(id);
        return NoContent();
    }
}