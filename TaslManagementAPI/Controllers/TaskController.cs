using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaslManagementAPI.DTOs;
using TaslManagementAPI.Interfaces;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;

        public TaskController(IUserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
        }

        [HttpPost("CreateTask")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] UserTaskDto taskDto)
        {
            await _userTaskService.AddTaskAsync(taskDto);
            return Ok("Task created successfully.");
        }

        [HttpGet("GetTaskById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _userTaskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPut("UpdateTask")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromBody] UserTaskDto updatedTask)
        {
            await _userTaskService.UpdateTaskAsync(updatedTask);
            return Ok("Task updated successfully.");
        }

        [HttpDelete("DeleteTask/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _userTaskService.DeleteTaskAsync(id);
            return Ok("Task deleted successfully.");
        }

        [HttpGet("SearchTasks")]
        [Authorize]
        public async Task<IActionResult> SearchTasks([FromQuery] string? title, [FromQuery] int? status, [FromQuery] int? priority)
        {
            var tasks =await _userTaskService.SearchTasksAsync(title, status, priority);
            return Ok(tasks);
        }

        [HttpGet("GetTasksByStatus/{status}")]
        [Authorize]
        public async Task<IActionResult> GetTasksByStatus(int status)
        {
            var tasks =await _userTaskService.GetTasksByStatusAsync(status);

            return Ok(tasks);
        }

        [HttpGet("GetTasksDueToday")]
        [Authorize]
        public async Task<IActionResult> GetTasksDueToday()
        {
            var tasks =await _userTaskService.GetTasksDueTodayAsync();
            return Ok(tasks);
        }

        [HttpGet("GetUserTaskCounts")]
        [Authorize(Roles = RolesConstants.Admin)]
        public async Task<IActionResult> GetUserTaskCounts()
        {
            var result = await _userTaskService.GetUserTaskCountsAsync();
            return Ok(result);
        }
        [HttpGet("GetUserTasksPagination")]
        [Authorize(Roles = RolesConstants.Admin)]
        public async Task<IActionResult> GetUserTasksPagination([FromQuery] int? priority, [FromQuery] int? status, [FromQuery] DateTime? dueDate, [FromQuery] string orderBy = "CreationDate", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (tasks, totalCount) = await _userTaskService.GetUserTasksPaginationAsync(priority, status, dueDate, orderBy, pageNumber, pageSize);
            var response = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Tasks = tasks
            };
            return Ok(response);
        }
    }
}