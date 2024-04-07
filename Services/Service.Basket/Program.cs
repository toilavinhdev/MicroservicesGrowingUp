var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Map("/", () => "Service.Basket");

app.Run();