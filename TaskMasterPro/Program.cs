using TaskMasterPro.Core.Interfaces;
using TaskMasterPro.Core.Services;
using TaskMasterPro.Infrastructure.Repositories;
using TaskMasterPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();


var dbProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
builder.Services.AddDbContext<TaskDbContext>(options =>
{
    if (string.Equals(dbProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
    {
        var pgConn = builder.Configuration.GetConnectionString("Postgres")
                    ?? "Host=localhost;Port=5432;Database=taskmasterdb;Username=taskuser;Password=taskpass";
        options.UseNpgsql(pgConn, npgsql => npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null));
    }
    else
    {
        var sqlConn = builder.Configuration.GetConnectionString("SqlServer")
                     ?? "Server=(localdb)\\mssqllocaldb;Database=TaskMasterDb;Trusted_Connection=true;MultipleActiveResultSets=true";
        options.UseSqlServer(sqlConn, sql => sql.EnableRetryOnFailure());
    }
});

var app = builder.Build();


 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseAuthorization();
app.MapControllers();


app.MapGet("/test", () => "API работает!");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }