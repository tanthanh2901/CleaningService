﻿using MassTransit;
using MessageBus.IntegrationEvents;
using TaskerService.Entities;
using TaskerService.Services;

namespace TaskerService.Consumers
{
    public class UserBecameTaskerConsumer : IConsumer<TaskerCreatedEvent>
    {
        private readonly ITaskerService _taskerService;
        private readonly ILogger<UserBecameTaskerConsumer> _logger;

        public UserBecameTaskerConsumer(
            ITaskerService taskerService,
            ILogger<UserBecameTaskerConsumer> logger)
        {
            _taskerService = taskerService;
            _logger = logger;
        }

        public async  Task Consume(ConsumeContext<TaskerCreatedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing UserBecameTaskerEvent for user {UserId}", message.UserId);

                var existingTasker = await _taskerService.GetTaskerByUserId(message.UserId);

                if (existingTasker != null)
                {
                    _logger.LogInformation("Tasker already exists for user {UserId}", message.UserId);
                    return;
                }

                if (message.Avatar == null)
                    message.Avatar = "";

                // Create new tasker
                var tasker = new Tasker
                {
                    UserId = message.UserId,
                    FullName = message.FullName,
                    Email = message.Email,
                    Avatar = message.Avatar,
                    PhoneNumber = message.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    TaskerCategories = message.CategoryIds.Select(id => new TaskerCategory
                    {
                        CategoryId = id
                    }).ToList()
                };

                await _taskerService.CreateAsync(tasker);
                _logger.LogInformation("Successfully created tasker for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserBecameTaskerEvent for user {UserId}", message.UserId);
                throw;
            }
        }
    }
}
