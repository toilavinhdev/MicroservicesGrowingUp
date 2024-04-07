var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Map("/", () => "Service.Discount");

app.Run();