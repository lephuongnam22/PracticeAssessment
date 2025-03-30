using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PracticeAssessment.Commons;
using PracticeAssessment.Controllers;
using PracticeAssessment.Core.Models;
using PracticeAssessment.Services;
using System.Security.Claims;
using Xunit;

namespace PracticeAssessment.UnitTest.Controllers;

public class TaskControllerUnitTest
{
    private readonly TaskController _taskController;
    private Mock<ITaskService> mockTaskService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public TaskControllerUnitTest()
    {
        mockTaskService = new();
        _mockHttpContextAccessor = new();
        _taskController = new TaskController(mockTaskService.Object, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task CreateTask_ValidTaskModel_ReturnsOkResult()
    {
        // Arrange
        var taskModel = new TaskModel { Description = "test", Title = "test" };
        var userId = 1;
        SetupIdentity(userId);

        mockTaskService.Setup(
            x => x.CreateTaskAsync(taskModel, userId))
            .ReturnsAsync(OperationResult.Ok());

        // Act
        var result = await _taskController.CreateTask(taskModel);

        // Assert
        var okResult = result as StatusCodeResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTask_InvalidTaskModel_ReturnsUnauthorizedResult()
    {
        // Arrange
        var taskModel = new TaskModel { Description = "test", Title = "test" };
        var userId = 0;

        SetupIdentity(userId);

        // Act
        var result = await _taskController.CreateTask(taskModel);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;

        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTaskById_ValidTaskId_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var response = new TaskResponse { Description = "Test", Title = "Test" };
        mockTaskService.Setup(x => x.GetTaskByIdAsync(taskId))
            .ReturnsAsync(OperationResult.Ok(response));

        // Act
        var result = await _taskController.GetTaskById(taskId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task UpdateTask_ValidId_ReturnsOkResult()
    {
        // Arrange
        var id = 1;
        int userId = 1;
        SetupIdentity(userId);
        var updateTaskRequest = new UpdateTaskRequest { Title = "Title", Description = "Description" };
        var taskModel = new TaskModel { Description = "test", Title = "test" };

        mockTaskService.Setup(x => x.UpdateTask(id, updateTaskRequest, userId))
            .ReturnsAsync(OperationResult.Ok(taskModel));

        // Act
        var result = await _taskController.UpdateTask(id, updateTaskRequest);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        okResult.Value.Should().BeEquivalentTo(taskModel);
    }

    [Fact]
    public async Task DeleteTask_ValidId_ReturnsOkResult()
    {
        // Arrange
        var id = 1;
        mockTaskService.Setup(x => x.DeleteTask(id)).ReturnsAsync(OperationResult.Ok);

        // Act
        var result = await _taskController.DeleteTask(id);

        // Assert
        var okResult = result as StatusCodeResult;
        okResult.Should().NotBeNull();
        
    }

    [Fact]
    public async Task GetTaskByUserId_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var pagingResponse = new PagingResponse<TaskModel>
        {

            Page = 1,
            TotalRecords = 1,
            PageSize = 100,
            HasMore = false,
            Data = new List<TaskModel> { new TaskModel { Description = "test", Title = "test" } }
        };

        var taskByUserRequest = new TaskByUserRequest { Status = 1, PageSize = 100, PageNumber = 1 };
        mockTaskService.Setup(x => x.GetTaskByUserId(userId, taskByUserRequest))
            .ReturnsAsync(OperationResult.Ok(pagingResponse));

        // Act
        var result = await _taskController.GetTaskByUserId(userId, taskByUserRequest);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        okResult.Value.Should().BeEquivalentTo(pagingResponse);
    }

    private void SetupIdentity(int userId)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

        var identity = new ClaimsIdentity(claims, "TestAuthType");

        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
