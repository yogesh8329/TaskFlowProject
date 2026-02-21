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
    /// <summary>
/// Handles task management operations like create, update, delete and fetch tasks.
/// </summary>
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

        /// <summary>
        /// Creates a new task for the authenticated user.
        /// </summary>
        /// <param name="dto">Task details</param>
        /// <response code="201">Task created successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="401">Unauthorized</response>
        /// 

        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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


        /// <summary>
        /// Returns tasks of currently authenticated user.
        /// </summary>
        /// <response code="200">User tasks returned</response>
        /// <response code="401">Unauthorized</response>

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



        /// <summary>
        /// Returns paginated list of tasks.
        /// </summary>
        /// <param name="query">Pagination and filter parameters</param>
        /// <response code="200">Tasks returned successfully</response>
        /// 
        [ProducesResponseType(typeof(PagedResult<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    
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



        /// <summary>
        /// Updates an existing task by id.
        /// </summary>
        /// 
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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



        /// <summary>
        /// Soft deletes a task by id.
        /// </summary>
        /// 

        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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
