using TaskFlow.Api.Common;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(int userId, CreateTaskDto dto);
        Task<List<TaskResponseDto>> GetMyTasksAsync(int userId);
        Task<List<TaskResponseDto>> GetAllTasksAsync(int userId);
        Task<PagedResult<TaskResponseDto>> GetTasksAsync(int userId, TaskQueryDto query);
        Task<TaskResponseDto> UpdateTaskAsync(int userId, int taskId, UpdateTaskDto dto);
        Task DeleteTaskAsync(int userId, int taskId);
    }
}
