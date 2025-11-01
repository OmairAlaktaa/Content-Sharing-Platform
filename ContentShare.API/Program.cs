using ContentShare.API;
using ContentShare.API.Middlewares;
using ContentShare.Application;
using ContentShare.Infrastructure;
using ContentShare.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApi();

builder.WebHost.UseSentry(options =>
{
    options.Dsn = "https://ef574c7278f3685bc789f4cb7ce4a262@o4510285936132096.ingest.us.sentry.io/4510285938425856";
    options.Debug = true;
    options.TracesSampleRate = 1.0;
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    await SeedData.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSentryTracing();

app.Run();

public partial class Program { }
