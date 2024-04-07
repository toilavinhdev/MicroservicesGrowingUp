using Microsoft.EntityFrameworkCore;
using Service.Discount;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer().AddSwaggerGen();
services.AddDbContext<DataContext>(
   options => options.UseSqlServer("Server=localhost; Database=MicroservicesGrowingUp; Integrated Security= True;Trust Server Certificate= True"));
services.AddGrpc();

var app = builder.Build();
app.UseSwagger()
   .UseSwaggerUI(options =>
   {
      options.DocumentTitle = "Service.Discount";
   });
app.Map("/", () => "Service.Discount");
app.MapGrpcService<GreeterService>();
app.MapGrpcService<DiscountService>();

app.Run();