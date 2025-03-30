using AutoMapper;
using Moq;
using PraceticeAssesment.Entity;
using PraceticeAssesment.Entity.Models;
using PraceticeAssesment.Entity.Repositories;
using PracticeAssessment.Core.Models;
using PracticeAssessment.Mappers;
using PracticeAssessment.Services;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Net;

namespace PracticeAssessment.UnitTest.Services;

public class TaskServiceUnitTest
{
    private readonly Mock<ITaskCommentRepository> _taskCommentRepositoryMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IUnitOfWork<DatabaseContext>> _unitOfWorkMock;
    private readonly Mock<IRelUserTaskRepository> _userTaskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly TaskService _taskService;
    public TaskServiceUnitTest()
    {
        _taskCommentRepositoryMock = new();
        _taskRepositoryMock = new();
        _taskHistoryRepositoryMock = new();
        _notificationRepositoryMock = new();
        _serviceProviderMock = new();
        _unitOfWorkMock = new();
        _userTaskRepositoryMock = new();
        _userRepositoryMock = new();

        _serviceProviderMock
            .Setup(mock => mock.GetService(typeof(ITaskRepository)))
            .Returns(_taskRepositoryMock.Object);

        _serviceProviderMock
            .Setup(mock => mock.GetService(typeof(ITaskHistoryRepository)))
            .Returns(_taskHistoryRepositoryMock.Object);

        _serviceProviderMock
            .Setup(mock => mock.GetService(typeof(INotificationRepository)))
            .Returns(_notificationRepositoryMock.Object);

        _serviceProviderMock
            .Setup(mock => mock.GetService(typeof(IRelUserTaskRepository)))
            .Returns(_userTaskRepositoryMock.Object);

        _serviceProviderMock
           .Setup(mock => mock.GetService(typeof(IUserRepository)))
           .Returns(_userRepositoryMock.Object);

        _serviceProviderMock
          .Setup(mock => mock.GetService(typeof(ITaskCommentRepository)))
          .Returns(_taskCommentRepositoryMock.Object);

        _taskService = new TaskService(
            _unitOfWorkMock.Object,
            _serviceProviderMock.Object,
            CreateMapper()
        );
    }

    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_OperationResult_With_TaskResponse()
    {
        // Arrange
        var taskId = 1;
        var taskEntity = new TaskEntity
        {
            Id = taskId,
            Title = "Test Task",
            Description = "This is a test task",
            Status = 1,
            DueDate = new DateTime(2022, 1, 1)
        };

        var taskResponse = new TaskResponse
        {
            Id = taskId,
            Title = "Test Task",
            Description = "This is a test task",
            Status = 1,
            DueDate = new DateTime(2022, 1, 1),
            Comments = new List<CommentModel>()
            {
                new CommentModel { Id = 1, Comment = "Comment 1", UserId = 1, TaskId = 1 },
                new CommentModel { Id = 2, Comment = "Comment 2", UserId = 2, TaskId = 1 }
            },
            Notifications = new List<NotificationModel>()
            {
                new NotificationModel { Id = 1, Message = "Notification 1", TaskId = 1, UserId = 1 },
                new NotificationModel { Id = 2, Message = "Notification 2", TaskId = 1, UserId = 2 }
            },
            TaskHistoryModels = new List<TaskHistoryModel>
            {
                new TaskHistoryModel { Id = 1, TaskId = 1, OldStatus = 0, NewStatus = 1 },
                new TaskHistoryModel { Id = 2, TaskId = 1, OldStatus = 1, NewStatus = 2 }
            }
        };

        var taskCommentEntities = new List<TaskCommentEntity>
            {
                new TaskCommentEntity { Id = 1, TaskId = taskId, Comment = "Comment 1", UserId = 1 },
                new TaskCommentEntity { Id = 2, TaskId = taskId, Comment = "Comment 2", UserId = 2 }
            };
        var taskHistoryEntities = new List<TaskHistory>
            {
                new TaskHistory { Id = 1, TaskId = taskId, OldStatus = 0, NewStatus = 1 },
                new TaskHistory { Id = 2, TaskId = taskId, OldStatus = 1, NewStatus = 2 }
            };
        var notificationEntities = new List<NotificationEntity>
            {
                new NotificationEntity { Id = 1, TaskId = taskId, Message = "Notification 1", UserId = 1 },
                new NotificationEntity { Id = 2, TaskId = taskId, Message = "Notification 2", UserId = 2 }
            };

        _taskRepositoryMock.Setup(r => r.GetById(taskId))
            .ReturnsAsync(taskEntity);

        _taskCommentRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<TaskCommentEntity, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskCommentEntities);

        _taskHistoryRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<TaskHistory, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskHistoryEntities);

        _notificationRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<NotificationEntity, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificationEntities);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.True(result.Succeeded);
        result.Value.Should().BeEquivalentTo(taskResponse);
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Return_OperationResult_With_TaskModel()
    {
        // Arrange
        var taskModel = new TaskModel
        {
            Title = "Test Task",
            Description = "This is a test task",
            Status = 1,
            DueDate = new DateTime(2022, 1, 1),
            AssignUserIds = new List<int> { 1, 2 }
        };

        var userId = 1;

        var taskResponse = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            Description = "This is a test task",
            Status = 1,
            DueDate = new DateTime(2022, 1, 1)
        };

        _userRepositoryMock.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<UserEntity, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync([new()
            {
                Id = 1
            },
            new()
            {
                Id = 2,
            }]);

        // Act
        var result = await _taskService.CreateTaskAsync(taskModel, userId);

        // Assert
        Assert.True(result.Succeeded);

        _taskRepositoryMock.Verify(r => r.Insert(It.IsAny<TaskEntity>()), Times.Once);

        _userTaskRepositoryMock.Verify(r => r.Insert(It.IsAny<RelUserTaskEntity>()), Times.Exactly(2));

        _notificationRepositoryMock.Verify(r => r.Insert(It.IsAny<NotificationEntity>()), Times.Exactly(2));
    }

    [Fact]
    public async Task DeleteTask_Should_Return_OperationResult()
    {
        // Arrange
        var taskId = 1;
        var taskEntity = new TaskEntity
        {
            Id = taskId,
            Title = "Test Task",
            Description = "This is a test task",
            Status = 1,
            DueDate = new DateTime(2022, 1, 1)
        };

        _taskRepositoryMock.Setup(r => r.GetById(taskId)).ReturnsAsync(taskEntity);

        _userTaskRepositoryMock.Setup(r =>
            r.GetAsync(It.IsAny<Expression<Func<RelUserTaskEntity, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync([new()
            {
                TaskId = taskId,
                UserId = 1
            },
            new()
            {
                TaskId = taskId,
                UserId = 2
            }]);

        _taskCommentRepositoryMock.Setup(r =>
        r.GetAsync(It.IsAny<Expression<Func<TaskCommentEntity, bool>>>(),
        It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new()
                {
                    TaskId = taskId,
                },
            ]);

        _notificationRepositoryMock.Setup(r =>
            r.GetAsync(It.IsAny<Expression<Func<NotificationEntity, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync([new()
            {
                TaskId = taskId
            }]);

        _taskHistoryRepositoryMock.Setup(r =>
            r.GetAsync(It.IsAny<Expression<Func<TaskHistory, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync([new()
            {
                TaskId = taskId
            }]);

        // Act
        var result = await _taskService.DeleteTask(taskId);

        // Assert
        Assert.True(result.Succeeded);

        _taskRepositoryMock.Verify(r => r.Delete(taskEntity), Times.Once);

        _taskCommentRepositoryMock.Verify(r => r.Delete(It.IsAny<TaskCommentEntity>())
        , Times.Once);

        _notificationRepositoryMock.Verify(r => r.Delete(It.IsAny<NotificationEntity>()), Times.Once);

        _taskHistoryRepositoryMock.Verify(r => r.Delete(It.IsAny<TaskHistory>()), Times.Once);

        _userTaskRepositoryMock.Verify(r => r.Delete(It.IsAny<RelUserTaskEntity>()), Times.Exactly(2));

        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_Should_Return_NotFoundResult()
    {
        int taskId = 1;
        // Act
        var result = await _taskService.DeleteTask(taskId);

        _taskRepositoryMock.Setup(r => r.GetById(taskId)).ReturnsAsync((TaskEntity)null!);

        // Assert
        Assert.False(result.Succeeded);
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static Mapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile(new ResponseProfiles()); }));
    }
}
