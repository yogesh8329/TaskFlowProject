using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;
using TaskFlow.Api.Common;
using TaskFlow.Api.Common.Enums;
using TaskFlow.Api.Common.Exceptions;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Repositories;

namespace TaskFlow.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskService> _logger;
        // private readonly IDistributedCache _cache; IDistributedCache cache

        public TaskService(
            AppDbContext context,
            IUnitOfWork unitOfWork,
            ILogger<TaskService> logger)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _logger = logger;
            //_cache = cache;
        }

        private string GetAllTasksCacheKey(int userId)
            => $"tasks:user:{userId}:all";

        // ================= CREATE =================
        public async Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = TaskItemStatus.Pending,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

           // await _cache.RemoveAsync(GetAllTasksCacheKey(userId));

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
               // RowVersion = task.RowVersion
            };
        }

        // ================= GET ALL =================
        public async Task<List<TaskResponseDto>> GetAllTasksAsync(int userId)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => new TaskResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Status = x.Status,
                    //RowVersion = x.RowVersion
                })
                .ToListAsync();
        }

        // ================= GET MY =================
        public async Task<List<TaskResponseDto>> GetMyTasksAsync(int userId)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => new TaskResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Status = x.Status,
                   // RowVersion = x.RowVersion
                })
                .ToListAsync();
        }

        // ================= PAGED =================
        public async Task<PagedResult<TaskResponseDto>> GetTasksAsync(int userId, TaskQueryDto query)
        {
            var tasksQuery = _context.Tasks
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted);

            if (query.Status.HasValue)
                tasksQuery = tasksQuery.Where(x => x.Status == query.Status.Value);

            var totalCount = await tasksQuery.CountAsync();

            var items = await tasksQuery
    .OrderByDescending(x => x.Id)
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .Select(x => new TaskResponseDto
    {
        Id = x.Id,
        Title = x.Title,
        Description = x.Description,
        Status = x.Status
    })
    .ToListAsync();

            return new PagedResult<TaskResponseDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        // ================= UPDATE =================
        public async Task<TaskResponseDto> UpdateTaskAsync(
      int userId,
      int taskId,
      UpdateTaskDto dto)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == taskId && !x.IsDeleted);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (task.UserId != userId)
                throw new ForbiddenException("Unauthorized");

            // Capture OLD values
            var oldValues = JsonSerializer.Serialize(new
            {
                task.Title,
                task.Description,
                task.Status
            });

            //// 🔥 VERY IMPORTANT — Set Original RowVersion BEFORE update
            //_context.Entry(task)
            //    .Property(x => x.RowVersion)
            //    .OriginalValue = dto.RowVersion;

            // Update fields
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            // Capture NEW values (after update, before save)
            var newValues = JsonSerializer.Serialize(new
            {
                task.Title,
                task.Description,
                task.Status
            });

            // Add Audit BEFORE Save
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = AuditAction.Update,
                EntityName = "Task",
                EntityId = task.Id,
                OldValues = oldValues,
                NewValues = newValues,
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                await _unitOfWork.SaveChangesAsync();   // ✅ Only ONE Save
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("This record was modified by another user.");
            }

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                // 🔥 Return updated version
            };
        }


        // ================= DELETE =================
        public async Task DeleteTaskAsync(int userId, int taskId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == taskId && !x.IsDeleted);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (task.UserId != userId)
                throw new ForbiddenException("Unauthorized");

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
