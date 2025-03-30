using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PraceticeAssesment.Entity;
using PraceticeAssesment.Entity.Models;
using PraceticeAssesment.Entity.Repositories;
using PracticeAssessment.Commons;
using PracticeAssessment.Core.Models;

namespace PracticeAssessment.Services;

public interface ITaskService
{
    Task<OperationResult<TaskResponse>> GetTaskByIdAsync(int taskId);

    Task<OperationResult> CreateTaskAsync(TaskModel taskModel, int userId);

    Task<OperationResult> DeleteTask(int id);

    Task<OperationResult<TaskModel>> UpdateTask(int id, UpdateTaskRequest updateTaskRequest, int userId);

    Task<OperationResult<PagingResponse<TaskModel>>> GetTaskByUserId(int userId, TaskByUserRequest taskByUserRequest);
}

public class TaskService(IUnitOfWork<DatabaseContext> unitOfWork,
    IServiceProvider serviceProvider,
    IMapper mapper) : ITaskService
{
    private readonly ITaskRepository _taskRepository = serviceProvider.GetRequiredService<ITaskRepository>();
    private readonly IRelUserTaskRepository _relUserTaskRepository = serviceProvider.GetRequiredService<IRelUserTaskRepository>();
    private readonly IUserRepository _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
    private readonly ITaskHistoryRepository _taskHistoryRepository = serviceProvider.GetRequiredService<ITaskHistoryRepository>();
    private readonly ITaskCommentRepository _taskCommentRepository = serviceProvider.GetRequiredService<ITaskCommentRepository>();
    private readonly INotificationRepository _notificationRepository = serviceProvider.GetRequiredService<INotificationRepository>();

    public async Task<OperationResult> CreateTaskAsync(TaskModel taskModel, int userId)
    {
        var taskEntity = mapper.Map<TaskEntity>(taskModel);

        if (taskModel.AssignUserIds != null && taskModel.AssignUserIds.Any())
        {
            var addRelUserTaskResult = await AddRelUserTask(taskEntity, taskModel.AssignUserIds, false);

            if (!addRelUserTaskResult.Succeeded)
            {
                return addRelUserTaskResult;
            }

            AddNotifications(taskEntity, userId, taskModel.AssignUserIds, true);
        }

        taskEntity.CreatedDate = DateTime.UtcNow;
        taskEntity.ModifiedDate = DateTime.UtcNow;
        taskEntity.CreatedBy = userId;
        taskEntity.ModifiedBy = userId;

        _taskRepository.Insert(taskEntity);

        AddTaskHistory(taskEntity, userId, 0, 1);

        await unitOfWork.SaveChangesAsync();

        return OperationResult.Ok();
    }

    public async Task<OperationResult<TaskResponse>> GetTaskByIdAsync(int taskId)
    {
        var entity = await _taskRepository.GetById(taskId);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        var taskModel = mapper.Map<TaskResponse>(entity);

        var taskComments = await _taskCommentRepository.GetAsync(n => n.TaskId == taskId);

        var taskHistorys = await _taskHistoryRepository.GetAsync(n => n.TaskId == taskId);

        var taskNotifications = await _notificationRepository.GetAsync(n => n.TaskId == taskId);

        taskModel.TaskHistoryModels = mapper.Map<List<TaskHistoryModel>>(taskHistorys);

        taskModel.Comments = mapper.Map<List<CommentModel>>(taskComments);

        taskModel.Notifications = mapper.Map<List<NotificationModel>>(taskNotifications);

        return OperationResult.Ok(taskModel);
    }

    public async Task<OperationResult> DeleteTask(int id)
    {
        var entity = await _taskRepository.GetById(id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        await DeleteRelUserTask(id);

        await DeleteTaskComment(id);

        await DeleteTaskNotification(id);

        await DeleteTaskHistory(id);

        _taskRepository.Delete(entity);

        await unitOfWork.SaveChangesAsync();

        return OperationResult.Ok();
    }

    public async Task<OperationResult<TaskModel>> UpdateTask(int id, UpdateTaskRequest updateTaskRequest, int userId)
    {
        var entity = await _taskRepository.GetById(id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        var addRelUserTaskResult = await AddRelUserTask(entity, updateTaskRequest.AssignUserIds, true);

        if (addRelUserTaskResult.Succeeded == false)
        {
            return addRelUserTaskResult;
        }

        if (entity.Status != updateTaskRequest.Status)
        {
            AddTaskHistory(entity, entity.Status, updateTaskRequest.Status, userId);
            entity.Status = updateTaskRequest.Status;
        }

        AddNotifications(entity, userId, updateTaskRequest.AssignUserIds, false);

        entity.Title = updateTaskRequest.Title;
        entity.Description = updateTaskRequest.Description;
        entity.DueDate = updateTaskRequest.DueDate;

        _taskRepository.Update(entity);

        await unitOfWork.SaveChangesAsync();

        return OperationResult.Ok(mapper.Map<TaskModel>(entity));
    }

    public async Task<OperationResult<PagingResponse<TaskModel>>> GetTaskByUserId(int userId, TaskByUserRequest taskByUserRequest)
    {
        var query = from t in _taskRepository.GetQueryable()
                    join rel in _relUserTaskRepository.GetQueryable()
                    on t.Id equals rel.TaskId
                    where rel.UserId == userId
                    select t;

        if (taskByUserRequest.Status != 0)
        {
            query = query.Where(n => n.Status == taskByUserRequest.Status);
        }

        if (taskByUserRequest.FromDate.HasValue)
        {
            query = query.Where(n => n.DueDate >= taskByUserRequest.FromDate);
        }

        if (taskByUserRequest.ToDate.HasValue)
        {
            query = query.Where(n => n.DueDate <= taskByUserRequest.ToDate);
        }

        var totalCount = query.Count();

        var pageResponse = new PagingResponse<TaskModel>
        {
            Page = taskByUserRequest.PageNumber,
            PageSize = taskByUserRequest.PageSize,
            TotalRecords = totalCount,
            HasMore = (totalCount / taskByUserRequest.PageSize) > taskByUserRequest.PageNumber - 1
        };

        var tasks = await query.Skip((taskByUserRequest.PageNumber - 1) * taskByUserRequest.PageSize)
            .Take(taskByUserRequest.PageSize)
            .ToListAsync();

        pageResponse.Data = mapper.Map<List<TaskModel>>(tasks);

        return pageResponse;
    }

    private void AddNotifications(TaskEntity taskEntity, int createUserId, IEnumerable<int> userIds, bool isAdd)
    {
        if (userIds == null || !userIds.Any())
            return;

        foreach (var userId in userIds)
        {
            string? message;

            if (isAdd)
            {
                message = string.Format("User {0} has added Task {1}", createUserId, taskEntity.Title);
            }
            else
            {
                message = string.Format("User {0} has updated Task {1}", createUserId, taskEntity.Title);
            }

            var notification = new NotificationEntity
            {
                Task = taskEntity,
                Message = message,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                HasNotified = false
            };

            _notificationRepository.Insert(notification);
        }
    }

    private void AddTaskHistory(TaskEntity entity, int oldStatus, int newStatus, int userId)
    {
        var taskHistory = new TaskHistory
        {
            Task = entity,
            TaskId = entity.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedDate = DateTime.UtcNow,
            UserId = userId
        };

        _taskHistoryRepository.Insert(taskHistory);
    }

    private async Task<OperationResult> AddRelUserTask(TaskEntity entity,
        IEnumerable<int> userIds, bool isRemoveOldRelation)
    {
        if (userIds == null || !userIds.Any())
        {
            return OperationResult.Ok();
        }

        if (isRemoveOldRelation)
        {
            var relUserTasks = await _relUserTaskRepository.GetAsync(n => n.TaskId == entity.Id);

            if (relUserTasks != null && relUserTasks.Count != 0)
            {
                foreach (var relUserTask in relUserTasks)
                {
                    _relUserTaskRepository.Delete(relUserTask);
                }
            }
        }

        var users = await _userRepository.GetAsync(n => userIds.Distinct().Contains(n.Id));

        if (users == null || users.Count == 0)
        {
            return OperationResult.BadRequest($"User with UserIds {string.Join(',', userIds)} not found");
        }

        foreach (var user in users)
        {
            var relUserTaskEntity = new RelUserTaskEntity
            {
                Task = entity,
                UserId = user.Id
            };

            _relUserTaskRepository.Insert(relUserTaskEntity);
        }

        return OperationResult.Ok();
    }

    private async Task DeleteRelUserTask(int taskId)
    {
        var relUserTasks = await _relUserTaskRepository
            .GetAsync(n => n.TaskId == taskId);

        if (relUserTasks != null && relUserTasks.Count != 0)
        {
            foreach (var relUserTask in relUserTasks)
            {
                _relUserTaskRepository.Delete(relUserTask);
            }
        }
    }

    private async Task DeleteTaskComment(int taskId)
    {
        var comments = await _taskCommentRepository
            .GetAsync(e => e.TaskId == taskId);

        if (comments != null && comments.Count != 0)
        {
            foreach (var comment in comments)
            {
                _taskCommentRepository.Delete(comment);
            }
        }
    }

    private async Task DeleteTaskNotification(int taskId)
    {
        var notifications = await _notificationRepository.
            GetAsync(e => e.TaskId == taskId);

        if (notifications != null && notifications.Count != 0)
        {
            foreach (var notification in notifications)
            {
                _notificationRepository.Delete(notification);
            }
        }
    }

    private async Task DeleteTaskHistory(int taskId)
    {
        var taskHistories = await _taskHistoryRepository
            .GetAsync(e => e.TaskId == taskId);

        if (taskHistories != null && taskHistories.Count != 0)
        {
            foreach (var taskHistory in taskHistories)
            {
                _taskHistoryRepository.Delete(taskHistory);
            }
        }
    }
}
