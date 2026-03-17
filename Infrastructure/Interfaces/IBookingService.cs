using System;
using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBookingService
{

    Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId);
    Task<int> CreateBookingAsync(CreateBookingRequest request);
    Task<bool> UpdateBookingStatusAsync(int id, string status);
    Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date);
    Task<BookingStatistics> GetStatisticsAsync();

}
