var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add configurations
builder.Services.AddConfigureSettings();
// Add external services to the container
builder.Services.AddExternalServices();
// Add services to the container
builder.Services.AddAppServices();

var app = builder.Build();

app.UseShaamSwaggerUI(builder, (options) => { });

//app.UseHttpsRedirection();

// Dynamic - Will register ExceptionsEndpoint and everything that implements IEndpointModule
var group = app.RegisterEndPoints();

group.MapDefaultEndpoints();

app.AddHeaders();

// Use middleware
app.UseMiddleware();

app.Run();