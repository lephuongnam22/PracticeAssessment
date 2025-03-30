using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PraceticeAssesment.Entity.Models;
using PracticeAssessment.Core.Models;
using PracticeAssessment.Services;

namespace PracticeAssessment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager, ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new UserEntity { Email = model.Email, UserName = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            return Ok(new { Result = "User created successfully" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound();

            var roles = await userManager.GetRolesAsync(user!);
            var token = tokenService.GenerateToken(user, roles);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}
