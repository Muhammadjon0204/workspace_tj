using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomController> _logger;

    public RoomController(IRoomService roomService, ILogger<RoomController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return NotFound($"Комната с id {id} не найдена");
        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(Room room)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = await _roomService.CreateAsync(room);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Room room)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _roomService.UpdateAsync(id, room);
        if (!result) return NotFound($"Комната с id {id} не найдена");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roomService.DeleteAsync(id);
        if (!result) return NotFound($"Комната с id {id} не найдена");
        return NoContent();
    }
}