using GameInventoryApi.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalException");

        if (feature?.Error is not null)
        {
            logger.LogError(feature.Error, "Unhandled exception occurred.");
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Server error",
            detail: "Unexpected error occurred. Please try again."
        ).ExecuteAsync(context);
    });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.EnsureCreated();

    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS TransactionLogs (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            Type TEXT NOT NULL,
            ItemId INTEGER NULL,
            Quantity INTEGER NOT NULL,
            GoldDelta INTEGER NOT NULL,
            CreatedAt TEXT NOT NULL
        );
    ");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
