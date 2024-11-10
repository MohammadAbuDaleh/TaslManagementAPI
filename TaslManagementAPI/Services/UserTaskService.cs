using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;
using TaslManagementAPI.DTOs;
using TaslManagementAPI.Interfaces;
using TaslManagementAPI.Models;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;
        public UserTaskService(IUserTaskRepository userTaskRepository, UserService userService)
        {
            _userTaskRepository = userTaskRepository;
            _userService = userService;
        }

        public async Task<UserTaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _userTaskRepository.GetTaskByIdAsync(id);
            if (task == null)  new Exception("The task not exists!"); 

            if (task.UserId != _userService.UserId &&! _userService.IsUserAdmin)
                throw new UnauthorizedAccessException("You do not have permission to access this task!");


            return new UserTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                UserId=task.User.Id
            };
        }

        public async Task AddTaskAsync(UserTaskDto taskDto)
        {
            if (taskDto.UserId!=_userService.UserId &&! _userService.IsUserAdmin) 
                throw new UnauthorizedAccessException("Logged in user must has ADMIN role to add task to another user!");
            
            var task = new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                DueDate = taskDto.DueDate,
                UserId = taskDto.UserId
            };
            await _userTaskRepository.AddTaskAsync(task);
        }

        public async Task UpdateTaskAsync(UserTaskDto taskDto)
        {
            if (taskDto.UserId != _userService.UserId && !_userService.IsUserAdmin)
                throw new UnauthorizedAccessException("Logged in user must has ADMIN role to update task to another user!");
            
            var task = await _userTaskRepository.GetTaskByIdAsync(taskDto.Id.Value);
            if (task == null) new Exception("The task not exists!");

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.Status = taskDto.Status;
            task.Priority = taskDto.Priority;
            task.DueDate = taskDto.DueDate;
            task.UserId = taskDto.UserId;


            await _userTaskRepository.UpdateTaskAsync(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _userTaskRepository.DeleteTaskAsync(id);
        }

        public async Task<List<UserTaskResultDto>> SearchTasksAsync(string title, int? status, int? priority)
        { 
        
            return await _userTaskRepository.SearchTasksAsync(title, status, priority);
        }
        public async Task<List<UserTaskCountDto>> GetUserTaskCountsAsync()
        {
            return await _userTaskRepository.GetUserTaskCountsAsync();
        }
        public async Task<List<UserTask>> GetTasksDueTodayAsync()
        {
            return await _userTaskRepository.GetTasksDueTodayAsync();
        }
        public async Task<List<UserTask>> GetTasksByStatusAsync(int status)
        {
            return await _userTaskRepository.GetTasksByStatusAsync(status);
        }

        public async Task<(IEnumerable<UserTaskResultDto> Tasks, int TotalCount)> GetUserTasksPaginationAsync(int? priority, int? status, DateTime? dueDate, string orderBy, int pageNumber, int pageSize)
        {
            var (tasks, totalCount) = await _userTaskRepository.GetUserTasksPaginationAsync(priority, status, dueDate, orderBy, pageNumber, pageSize);
            var taskDtos = tasks.Select(MapToDto);
            return (taskDtos, totalCount);
        }

        private UserTaskResultDto MapToDto(UserTask task)
        {
            return new UserTaskResultDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = Enum.GetName(typeof(Status), task.Status),
                Priority = Enum.GetName(typeof(Priority), task.Priority),
                DueDate = task.DueDate,
                CreationDate=task.CreationDate,
                UserId=task.UserId,
                UserName=task.User?.UserName
            };
        }
    }
}
