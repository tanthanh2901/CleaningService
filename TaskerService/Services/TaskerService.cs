using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Dtos;
using TaskerService.Entities;
using TaskerService.Repositories;
using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.IdentityModel.Tokens;

namespace TaskerService.Services
{
    public class TaskerService : ITaskerService
    {
        private readonly TaskerDbContext dbContext;
        private readonly ITaskerRepository taskerRepository;
        private readonly ICatalogService catalogService;
        private readonly IMapper mapper;
        private readonly ILogger<TaskerService> _logger;
        private readonly IEventBus eventBus;

        public TaskerService(
            TaskerDbContext dbContext,
            IMapper mapper,
            ITaskerRepository taskerRepository,
            ICatalogService catalogService,
            ILogger<TaskerService> logger,
            IEventBus eventBus)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.taskerRepository = taskerRepository;
            this.catalogService = catalogService;
            this._logger = logger;
            this.eventBus = eventBus;
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

        public async Task<bool> ConfirmBooking(int bookingId)
        {
            try
            {
                var booking = await dbContext.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    _logger.LogError("Booking {BookingId} not found", bookingId);
                    return false;
                }

                var tasker = await dbContext.Taskers
                    .FirstOrDefaultAsync(t => t.TaskerId == booking.TaskerId);

                if (tasker == null)
                {
                    _logger.LogError("Tasker {TaskerId} not found", booking.TaskerId);
                    return false;
                }

                tasker.IsAvailable = false;

                booking.BookingStatus = "Confirmed";
                await dbContext.SaveChangesAsync();

                // Publish event to update booking status
                await eventBus.PublishAsync(new BookingStatusChangedEvent
                {
                    BookingId = booking.BookingId,
                    NewStatus = booking.BookingStatus,
                    ChangedAt = DateTime.UtcNow
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming booking {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<bool> CompletedBooking(int bookingId)
        {
            try
            {
                var booking = await dbContext.Bookings
                    .Include(b => b.Tasker)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    _logger.LogError("Booking {BookingId} not found", bookingId);
                    return false;
                }

                // Update booking status
                booking.BookingStatus = "Completed";
                booking.CompletedAt = DateTime.UtcNow;

                // Update tasker stats
                var tasker = booking.Tasker;
                tasker.CompletedTasks++;
                tasker.IsAvailable = true;  // Make tasker available again

                await dbContext.SaveChangesAsync();

                // Publish event to update booking status
                await eventBus.PublishAsync(new BookingStatusChangedEvent
                {
                    BookingId = booking.BookingId,
                    NewStatus = booking.BookingStatus,
                    ChangedAt = DateTime.UtcNow
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing booking {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<bool> StartBooking(int bookingId)
        {
            try
            {
                var booking = await dbContext.Bookings
                    .Include(b => b.Tasker)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    _logger.LogError("Booking {BookingId} not found", bookingId);
                    return false;
                }

                // Update booking status
                booking.BookingStatus = "InProgress";
                booking.CompletedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                // Publish event to update booking status
                await eventBus.PublishAsync(new BookingStatusChangedEvent
                {
                    BookingId = booking.BookingId,
                    NewStatus = booking.BookingStatus,
                    ChangedAt = DateTime.UtcNow
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing booking {BookingId}", bookingId);
                return false;
            }
        }


        public async Task<List<Booking>> GetTaskerBookings(int taskerId)
        {
            var query = dbContext.Bookings
                .Where(b => b.TaskerId == taskerId);

            return await query
                .OrderByDescending(b => b.ScheduleTime)
                .ToListAsync();
        }



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
    }
}
