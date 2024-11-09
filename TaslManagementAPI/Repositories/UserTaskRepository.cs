using Microsoft.EntityFrameworkCore;
using TaslManagementAPI.DTOs;
using TaslManagementAPI.Infrastructure;
using TaslManagementAPI.Interfaces;
using TaslManagementAPI.Models;
using TaslManagementAPI.Services;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Repositories
{

    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserService _userService;

        public UserTaskRepository(ApplicationDBContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<UserTask> GetTaskByIdAsync(int id)
        {
            return await _context.UserTasks.FindAsync(id);
        }

        public async Task AddTaskAsync(UserTask task)
        {
            await _context.UserTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.UserTasks.FindAsync(id);
            if (task != null)

                if (task.UserId != _userService.UserId && !_userService.IsUserAdmin)
                    throw new UnauthorizedAccessException("You do not have permission to access this task.");

            _context.UserTasks.Remove(task);
            await _context.SaveChangesAsync();

        }


        public async Task<List<UserTaskResultDto>> SearchTasksAsync(string title, int? status, int? priority)
        {//i have choose stored procedure because :performance better and little data retrived from DB
            var userTask = new List<UserTaskResultDto>();
            var query = "EXEC TaskManagementDB.SearchUserTasks";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;

                    var titleParam = command.CreateParameter();
                    titleParam.ParameterName = "@Title";
                    titleParam.Value = title ?? (object)DBNull.Value;
                    command.Parameters.Add(titleParam);

                    var statusParam = command.CreateParameter();
                    statusParam.ParameterName = "@Status";
                    statusParam.Value = status ?? (object)DBNull.Value; 
                    command.Parameters.Add(statusParam);

                    var priorityParam = command.CreateParameter();
                    priorityParam.ParameterName = "@Priority";
                    priorityParam.Value = priority ?? (object)DBNull.Value;
                    command.Parameters.Add(priorityParam);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new UserTaskResultDto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Status = Enum.GetName(typeof(Status), reader.GetInt32(reader.GetOrdinal("Status"))),
                                Priority = Enum.GetName(typeof(Priority), reader.GetInt32(reader.GetOrdinal("Priority"))),
                                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName"))
                            };

                            userTask.Add(dto);
                        }
                    }
                }
            }
            return userTask;
        }
        public async Task<(List<UserTask> Tasks, int TotalCount)> GetUserTasksPaginationAsync(int? priority, int? status, DateTime? dueDate, string orderBy, int pageNumber, int pageSize)
        {
            var query = _context.UserTasks.Include(x=>x.User).AsQueryable();

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate.Date == dueDate.Value.Date);
            }

            query = orderBy switch
            {
                "CreationDate" => query.OrderBy(t => t.CreationDate),
                "Priority" => query.OrderBy(t => t.Priority),
                "DueDate" => query.OrderBy(t => t.DueDate),
                _ => query.OrderBy(t => t.CreationDate)
            };

            var totalCount = await query.CountAsync();

            var tasks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tasks, totalCount);
        }
        public async Task<List<UserTask>> GetTasksByStatusAsync(int status)
        {
            var tasks = _userService.IsUserAdmin
                ? await _context.UserTasks.FromSqlInterpolated($"EXEC TaskManagementDB.GetTasksByStatus {status}, NULL").ToListAsync()
                : await _context.UserTasks.FromSqlInterpolated($"EXEC TaskManagementDB.GetTasksByStatus {status}, {_userService.UserId}").ToListAsync();

            return tasks;
        }

        public async Task<List<UserTask>> GetTasksDueTodayAsync()
        {
            var tasks = await _context.UserTasks.FromSqlRaw("EXEC TaskManagementDB.GetTasksDueToday").ToListAsync();
            return tasks;
        }

        public async Task<List<UserTaskCountDto>> GetUserTaskCountsAsync()
        {
            var userTaskCounts = new List<UserTaskCountDto>();
            var query = "EXEC TaskManagementDB.GetUserTaskCounts";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new UserTaskCountDto
                            {
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                TaskCount = reader.GetInt32(reader.GetOrdinal("TaskCount"))
                            };

                            userTaskCounts.Add(dto);
                        }
                    }
                }
            }

            return userTaskCounts;
        }

    }
}
