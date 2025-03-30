using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PraceticeAssesment.Entity;
using PraceticeAssesment.Entity.Models;
using PraceticeAssesment.Entity.Repositories;
using PracticeAssessment.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options =>
              options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<UserEntity, IdentityRole<int>>(options => 
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = null!; // Allow any characters in the username
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;

}).AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();

var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Authentication failed. Exception: {Exception}", context.Exception);
            context.Response.Headers.Add("Token-Invalid", "true");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IRelUserTaskRepository, RelUserTaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
