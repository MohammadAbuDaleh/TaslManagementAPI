using TaslManagementAPI.DTOs;
using TaslManagementAPI.Models;

namespace TaslManagementAPI.Interfaces
{
    public interface IUserTaskService
    {
        Task<UserTaskDto> GetTaskByIdAsync(int id);
        Task AddTaskAsync(UserTaskDto task);
        Task UpdateTaskAsync(UserTaskDto taskDto);
        Task DeleteTaskAsync(int id);

        Task<List<UserTaskResultDto>> SearchTasksAsync(string title, int? status, int? priority);
        Task<List<UserTask>> GetTasksByStatusAsync(int status);
        Task<List<UserTask>> GetTasksDueTodayAsync();
        Task<List<UserTaskCountDto>> GetUserTaskCountsAsync();
       Task<(IEnumerable<UserTaskResultDto> Tasks, int TotalCount)> GetUserTasksPaginationAsync(int? priority, int? status, DateTime? dueDate, string orderBy, int pageNumber, int pageSize);

    }
}
