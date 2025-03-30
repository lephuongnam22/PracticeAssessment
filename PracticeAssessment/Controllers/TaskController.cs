using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeAssessment.Commons;
using PracticeAssessment.Core.Models;
using PracticeAssessment.Services;
using System.Security.Claims;

namespace PracticeAssessment.Controllers;

[Authorize(Roles = "Admin,Manager,User")]
[Route("api/tasks")]
[ApiController]
public class TaskController(ITaskService taskService,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
    {
        if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }

        return (await taskService.CreateTaskAsync(taskModel, userId)).ToActionResult();
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskById(int taskId)
    {
        return (await taskService.GetTaskByIdAsync(taskId)).ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest updateTaskRequest)
    {
        if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }

        return (await taskService.UpdateTask(id, updateTaskRequest, userId)).ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        return (await taskService.DeleteTask(id)).ToActionResult();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTaskByUserId(int userId, 
        [FromQuery] TaskByUserRequest taskByUserRequest)
    {
        return (await taskService.GetTaskByUserId(userId, taskByUserRequest)).ToActionResult();
    }
}
