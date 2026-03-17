using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/workspaces")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;
    private readonly ILogger<WorkspaceController> _logger;

    public WorkspaceController(IWorkspaceService workspaceService, ILogger<WorkspaceController> logger)
    {
        _workspaceService = workspaceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workspace>>> GetAll()
    {
        var workspaces = await _workspaceService.GetAllAsync();
        return Ok(workspaces);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Workspace>> GetById(int id)
    {
        var workspace = await _workspaceService.GetByIdAsync(id);
        if (workspace == null) return NotFound($"Рабочее место с id {id} не найдено");
        return Ok(workspace);
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<IEnumerable<Workspace>>> GetByRoom(int roomId)
    {
        var workspaces = await _workspaceService.GetByRoomAsync(roomId);
        return Ok(workspaces);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(Workspace workspace)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = await _workspaceService.CreateAsync(workspace);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Workspace workspace)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _workspaceService.UpdateAsync(id, workspace);
        if (!result) return NotFound($"Рабочее место с id {id} не найдено");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _workspaceService.DeleteAsync(id);
        if (!result) return NotFound($"Рабочее место с id {id} не найдено");
        return NoContent();
    }
}