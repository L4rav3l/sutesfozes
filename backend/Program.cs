using DotNetEnv;
using SutesFozes.Infrastructure;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactOnly",
        policy =>
        {
            policy.WithOrigins(Environment.GetEnvironmentVariable("PRODUCT_LINK"))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddSingleton<Postgresql>();
builder.Services.AddSingleton<Argon2>();
builder.Services.AddSingleton<JsonWebToken>();
builder.Services.AddSingleton<Mail>();

var app = builder.Build();

app.UseCors("ReactOnly");
app.MapControllers();
app.Run();