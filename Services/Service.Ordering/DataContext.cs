using Microsoft.EntityFrameworkCore;

namespace Service.Ordering;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; } = default!;
}