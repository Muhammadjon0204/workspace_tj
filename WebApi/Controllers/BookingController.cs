using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<Booking>>> GetCompanyBookings(int companyId)
    {
        var bookings = await _bookingService.GetCompanyBookingsAsync(companyId);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateBookingRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = await _bookingService.CreateBookingAsync(request);
        return Ok(id);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            return BadRequest("Статус не может быть пустым");

        var result = await _bookingService.UpdateBookingStatusAsync(id, request.Status);
        if (!result) return NotFound($"Бронирование с id {id} не найдено");
        return NoContent();
    }

    [HttpGet("daily")]
    public async Task<ActionResult<IEnumerable<Booking>>> GetByDate(DateTime date)
    {
        var bookings = await _bookingService.GetBookingsByDateAsync(date);
        return Ok(bookings);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<BookingStatistics>> GetStatistics()
    {
        var stats = await _bookingService.GetStatisticsAsync();
        return Ok(stats);
    }
}