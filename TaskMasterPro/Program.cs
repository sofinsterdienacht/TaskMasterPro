using TaskMasterPro.Core.Interfaces;
using TaskMasterPro.Core.Services;
using TaskMasterPro.Infrastructure.Repositories;
using TaskMasterPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TaskMasterDb;Trusted_Connection=true;MultipleActiveResultSets=true"));
var app = builder.Build();

 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();