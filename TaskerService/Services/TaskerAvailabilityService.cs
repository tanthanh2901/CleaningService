using AutoMapper;
using EventBus;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Services
{
    public class TaskerAvailabilityService : ITaskerAvailabilityService
    {
        private readonly TaskerDbContext _dbContext;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly ICatalogService _catalogService;
        private readonly ILogger<TaskerAvailabilityService> _logger;

        public TaskerAvailabilityService(
            TaskerDbContext dbContext,
            IEventBus eventBus,
            ILogger<TaskerAvailabilityService> logger,
            ICatalogService catalogService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _eventBus = eventBus;
            _logger = logger;
            _catalogService = catalogService;
            _mapper = mapper;
        }

        public async Task<bool> IsTaskerAvailableAsync(int taskerId, DateTime startTime, DateTime endTime)
        {
            var tasker = await _dbContext.Taskers.FindAsync(taskerId);
            if (tasker == null)
                return false;

            var conflictingSlots = await _dbContext.TaskerAvailabilitySlots
                .Where(slot => slot.TaskerId == taskerId &&
                              slot.SlotType != AvailabilitySlotType.Available &&
                              ((slot.StartTime <= startTime && slot.EndTime > startTime) ||
                               (slot.StartTime < endTime && slot.EndTime >= endTime) ||
                               (slot.StartTime >= startTime && slot.EndTime <= endTime)))
                .AnyAsync();

            return !conflictingSlots;
        }

        public async Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategoryAsync(
            int categoryId, DateTime startTime, DateTime endTime)
        {
            var category = await _catalogService.GetCategory(categoryId);
            if (category == null)
                throw new Exception("Category not found");

            var taskersInCategory = await _dbContext.Taskers
                .Include(t => t.TaskerCategories)
                .Where(t => t.TaskerCategories.Any(c => c.CategoryId == categoryId))
                .ToListAsync();

            var availableTaskers = new List<Tasker>();

            // Check availability for each tasker
            foreach (var tasker in taskersInCategory)
            {
                if (await IsTaskerAvailableAsync(tasker.TaskerId, startTime, endTime))
                {
                    availableTaskers.Add(tasker);
                }
            }

            return availableTaskers
                .OrderByDescending(t => t.RatingAverage)
                .Select(t => _mapper.Map<TaskerWithCategoriesDto>(t))
                .ToList();
        }

        public async Task CreateUnavailabilitySlotAsync(int taskerId, DateTime startTime, DateTime endTime, int bookingId, string reason)
        {
            var slot = new TaskerAvailabilitySlot
            {
                TaskerId = taskerId,
                StartTime = startTime,
                EndTime = endTime,
                SlotType = AvailabilitySlotType.Booked,
                BookingId = bookingId,
                Reason = reason,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.TaskerAvailabilitySlots.Add(slot);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created unavailability slot for tasker {TaskerId} from {StartTime} to {EndTime}",
                taskerId, startTime, endTime);
        }

        public async Task RemoveUnavailabilitySlotAsync(int bookingId)
        {
            var slot = await _dbContext.TaskerAvailabilitySlots
                .FirstOrDefaultAsync(s => s.BookingId == bookingId);

            if (slot != null)
            {
                _dbContext.TaskerAvailabilitySlots.Remove(slot);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Removed unavailability slot for booking {BookingId}", bookingId);
            }
        }

        public async Task<List<DateTime>> GetTaskerAvailableTimeSlotsAsync(int taskerId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            // Get all unavailable slots for the day
            var unavailableSlots = await _dbContext.TaskerAvailabilitySlots
                .Where(slot => slot.TaskerId == taskerId &&
                              slot.SlotType != AvailabilitySlotType.Available &&
                              slot.StartTime < endOfDay &&
                              slot.EndTime > startOfDay)
                .OrderBy(slot => slot.StartTime)
                .ToListAsync();

            // Generate available time slots (example: 8 AM to 8 PM, 1-hour slots)
            var availableSlots = new List<DateTime>();
            var workingHours = new { Start = 8, End = 20 }; // 8 AM to 8 PM

            for (int hour = workingHours.Start; hour < workingHours.End; hour++)
            {
                var slotStart = date.Date.AddHours(hour);
                var slotEnd = slotStart.AddHours(1);

                // Check if this slot conflicts with any unavailable slot
                bool isConflict = unavailableSlots.Any(us =>
                    (us.StartTime <= slotStart && us.EndTime > slotStart) ||
                    (us.StartTime < slotEnd && us.EndTime >= slotEnd) ||
                    (us.StartTime >= slotStart && us.EndTime <= slotEnd));

                if (!isConflict)
                {
                    availableSlots.Add(slotStart);
                }
            }

            return availableSlots;
        }
    }
}
