using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Dtos;
using TaskerService.Entities;
using TaskerService.Repositories;
using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.IdentityModel.Tokens;
using TaskerService.Models;
using MassTransit;
using MessageBus;

namespace TaskerService.Services
{
    public class TaskerService : ITaskerService
    {
        private readonly TaskerDbContext dbContext;
        private readonly ITaskerRepository taskerRepository;
        private readonly ICatalogService catalogService;
        private readonly IBookingService bookingService;
        private readonly IMapper mapper;
        private readonly ILogger<TaskerService> _logger;
        private readonly IEventBus eventBus;
        private readonly ITaskerAvailabilityService _availabilityService;

        public TaskerService(
            TaskerDbContext dbContext,
            IMapper mapper,
            ITaskerRepository taskerRepository,
            ICatalogService catalogService,
            ILogger<TaskerService> logger,
            IEventBus eventBus,
            IBookingService bookingService,
            ITaskerAvailabilityService availabilityService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.taskerRepository = taskerRepository;
            this.catalogService = catalogService;
            this._logger = logger;
            this.eventBus = eventBus;
            this.bookingService = bookingService;
            _availabilityService = availabilityService;
        }

        public async Task CreateAsync(Tasker tasker)
        {
            var existingTasker = await dbContext.Taskers
                .Include(t => t.TaskerCategories)
                .FirstOrDefaultAsync(t => t.UserId == tasker.UserId);

            if (existingTasker != null)
            {
                // Update existing tasker's categories
                var existingCategoryIds = existingTasker.TaskerCategories.Select(tc => tc.CategoryId).ToList();
                var newCategoryIds = tasker.TaskerCategories.Select(tc => tc.CategoryId).ToList();

                foreach (var categoryId in newCategoryIds.Except(existingCategoryIds))
                {
                    existingTasker.TaskerCategories.Add(new TaskerCategory
                    {
                        CategoryId = categoryId,
                        TaskerId = existingTasker.TaskerId
                    });
                }

                dbContext.Taskers.Update(existingTasker);
            }
            else
            {
                await taskerRepository.CreateTasker(tasker);
            }
        }

        //public async Task<bool> ConfirmBooking(int bookingId)
        //{
        //    try
        //    {
                
        //        var tasker = await dbContext.Taskers
        //            .FirstOrDefaultAsync(t => t.TaskerId == booking.TaskerId);

        //        if (tasker == null)
        //        {
        //            _logger.LogError("Tasker {TaskerId} not found", booking.TaskerId);
        //            return false;
        //        }

        //        tasker.IsAvailable = false;

        //        dbContext.Taskers.Update(tasker);

        //        await dbContext.SaveChangesAsync();

        //        // Publish event to update booking status
        //        await eventBus.PublishAsync(new BookingStatusChangedEvent
        //        {
        //            BookingId = booking.BookingId,
        //            NewStatus = booking.BookingStatus.ToString(),
        //            ChangedAt = booking.UpdatedAt
        //        });

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error confirming booking {BookingId}", bookingId);
        //        return false;
        //    }
        //}

        //public async Task<bool> CompletedBooking(int bookingId)
        //{
        //    try
        //    {
        //        var booking = await dbContext.Bookings
        //            .Include(b => b.Tasker)
        //            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        //        if (booking == null)
        //        {
        //            _logger.LogError("Booking {BookingId} not found", bookingId);
        //            return false;
        //        }

        //        // Update booking status
        //        booking.BookingStatus = BookingStatus.Completed;
        //        booking.CompletedAt = DateTime.Now;

        //        // Update tasker stats
        //        var tasker = booking.Tasker;
        //        tasker.CompletedTasks++;
        //        tasker.IsAvailable = true;  // Make tasker available again

        //        dbContext.Bookings.Update(booking);
        //        dbContext.Taskers.Update(tasker);

        //        await dbContext.SaveChangesAsync();

        //        // Publish event to update booking status
        //        await eventBus.PublishAsync(new BookingStatusChangedEvent
        //        {
        //            BookingId = booking.BookingId,
        //            NewStatus = booking.BookingStatus.ToString(),
        //            ChangedAt = DateTime.Now
        //        });

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error completing booking {BookingId}", bookingId);
        //        return false;
        //    }
        //}

        //public async Task<bool> StartBooking(int bookingId)
        //{
        //    try
        //    {
        //        var booking = await dbContext.Bookings
        //            .Include(b => b.Tasker)
        //            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        //        if (booking == null)
        //        {
        //            _logger.LogError("Booking {BookingId} not found", bookingId);
        //            return false;
        //        }

        //        // Update booking status
        //        booking.BookingStatus = BookingStatus.InProgress;
        //        booking.UpdatedAt = DateTime.Now;

        //        dbContext.Bookings.Update(booking);
        //        await dbContext.SaveChangesAsync();

        //        // Publish event to update booking status
        //        await eventBus.PublishAsync(new BookingStatusChangedEvent
        //        {
        //            BookingId = booking.BookingId,
        //            NewStatus = booking.BookingStatus.ToString(),
        //            ChangedAt = booking.UpdatedAt
        //        });

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error completing booking {BookingId}", bookingId);
        //        return false;
        //    }
        //}

        public async Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategory(int categoryId)
        {
            var category = await catalogService.GetCategory(categoryId);
            if (category == null)
                throw new Exception("Category not found");

            var taskers = await dbContext.Taskers
                .Include(t => t.TaskerCategories)
                .Where(t => t.IsAvailable &&
                           t.TaskerCategories.Any(c => c.CategoryId == categoryId))
                .OrderByDescending(t => t.RatingAverage)
                .ToListAsync();

            return mapper.Map<List<TaskerWithCategoriesDto>>(taskers);
        }

        public async Task<bool> SetAvailable(int userId)
        {
            var tasker = await taskerRepository.GetTaskerByUserId(userId);

            if (tasker != null)
            {
                bool currentState = tasker.IsAvailable;
                tasker.IsAvailable = !currentState;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<Tasker> GetTaskerByTaskerId(int taskerId)
        {
            return await taskerRepository.GetTaskerByTaskerId(taskerId);
        }

        public async Task<Tasker> GetTaskerByUserId(int UserId)
        {
            return await taskerRepository.GetTaskerByUserId(UserId);
        }

        public async Task<bool> UpdateBookingStatus(UpdateBookingStatusModel model)
        {
            var booking = await bookingService.GetBookingByIdAsync(model.BookingId);
            if (booking == null)
            {
                _logger.LogError("Booking {BookingId} not found", model.BookingId);
                return false;
            }
            var endTime = booking.BookingDuration.DurationHours;
            try
            {
                var tasker = await dbContext.Taskers
                    .FirstOrDefaultAsync(t => t.TaskerId == booking.TaskerId);

                if (tasker == null)
                {
                    _logger.LogError("Tasker {TaskerId} not found", booking.TaskerId);
                    return false;
                }

                var currentStatus = booking.BookingStatus;
                var newStatus = model.BookingStatus;

                switch (newStatus)
                {
                    case Shared.Enums.BookingStatus.Confirmed:
                        if (currentStatus == Shared.Enums.BookingStatus.Assigned)
                        {
                            // Create unavailability slot for the specific time period
                            await _availabilityService.CreateUnavailabilitySlotAsync(
                                booking.TaskerId,
                                booking.ScheduleTime,
                                booking.ScheduleTime.AddHours(booking.BookingDuration.DurationHours),
                                booking.BookingId,
                                "confirmed_booking");
                        }
                        break;

                    case Shared.Enums.BookingStatus.Completed:
                        if (currentStatus == Shared.Enums.BookingStatus.InProgress)
                        {
                            // Remove the unavailability slot - tasker is now available again
                            await _availabilityService.RemoveUnavailabilitySlotAsync(booking.BookingId);

                            // Update tasker stats
                            tasker.CompletedTasks++;
                            dbContext.Taskers.Update(tasker);
                        }
                        break;

                    case Shared.Enums.BookingStatus.Canceled:
                        // Remove unavailability slot if booking is cancelled
                        await _availabilityService.RemoveUnavailabilitySlotAsync(booking.BookingId);
                        break;

                    case Shared.Enums.BookingStatus.InProgress:
                        // No availability change needed
                        break;

                    default:
                        throw new ArgumentException($"Invalid booking status transition from {currentStatus} to {newStatus}");
                }

                await dbContext.SaveChangesAsync();

                // Publish booking status change event
                await eventBus.PublishAsync(new BookingStatusChangedEvent
                {
                    BookingId = booking.BookingId,
                    NewStatus = newStatus.ToString(),
                    ChangedAt = DateTime.UtcNow
                });

                _logger.LogInformation("Updated booking {BookingId} status from {OldStatus} to {NewStatus}",
                    model.BookingId, currentStatus, newStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status for {BookingId}", model.BookingId);
                return false;
            }
        }
    }
}
