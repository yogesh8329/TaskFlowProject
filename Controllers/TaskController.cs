using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Api.Common;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;


namespace TaskFlow.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new Exception("NameIdentifier claim missing");

            return int.Parse(claim.Value);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        { 
            var result = await _taskService.CreateTaskAsync(GetUserId(), dto);

            return Ok(new ApiResponse<TaskResponseDto>
            {
                Success = true,
                Message = "Task created successfully",
                Data = result
            });
        }
     

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> MyTasks()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var tasks = await _taskService.GetMyTasksAsync(userId);

            return Ok(new ApiResponse<List<TaskResponseDto>>
            {
                Success = true,
                Message = "Tasks fetched successfully",
                Data = tasks
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> AllTasks()
        {
            var userId = GetUserId();

            var tasks = await _taskService.GetAllTasksAsync(userId);

            return Ok(new ApiResponse<List<TaskResponseDto>>
            {
                Success = true,
                Message = "All tasks fetched successfully",
                Data = tasks
            });
        }




        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskQueryDto query)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _taskService.GetTasksAsync(userId, query);
            return Ok(new ApiResponse<PagedResult<TaskResponseDto>>
            {
                Success = true,
                Message = "Tasks fetched successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        { 
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _taskService.UpdateTaskAsync(userId, id, dto);

            return Ok(new ApiResponse<TaskResponseDto>
            {
                Success = true,
                Message = "Task updated successfully",
                Data = result
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            await _taskService.DeleteTaskAsync(userId, id);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Task deleted successfully"
            });
        }




    }
}
