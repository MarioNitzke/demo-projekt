var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureApp();

await DatabaseConfiguration.InitializeAsync(app.Services);

app.Run();
