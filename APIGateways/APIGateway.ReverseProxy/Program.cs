var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

var configurationRoot = new ConfigurationBuilder()
    .SetBasePath(environment.ContentRootPath)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json")
    .AddEnvironmentVariables()
    .Build();

var reverseProxy = configurationRoot.GetSection("ReverseProxy");

var services = builder.Services;
services.AddReverseProxy().LoadFromConfig(reverseProxy);

var app = builder.Build();
app.MapReverseProxy();
app.Map("/", () => "APIGateway.ReverseProxy");

app.Run();