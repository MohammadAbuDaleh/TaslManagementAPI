using TaslManagementAPI.DTOs;
using TaslManagementAPI.Models;

namespace TaslManagementAPI.Interfaces
{
    public interface IUserTaskRepository
    {
        Task<UserTask> GetTaskByIdAsync(int id);
        Task AddTaskAsync(UserTask task);
        Task UpdateTaskAsync(UserTask task);
        Task DeleteTaskAsync(int id);
        Task<List<UserTask>> GetTasksByStatusAsync(int status);
        Task<List<UserTask>> GetTasksDueTodayAsync();
        Task<List<UserTaskCountDto>> GetUserTaskCountsAsync();
        Task<List<UserTaskResultDto>> SearchTasksAsync(string title, int? status, int? priority);
        Task<(List<UserTask> Tasks, int TotalCount)> GetUserTasksPaginationAsync(int? priority, int? status, DateTime? dueDate, string orderBy, int pageNumber, int pageSize);
    }
}
