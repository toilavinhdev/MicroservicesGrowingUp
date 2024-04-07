using Microsoft.EntityFrameworkCore;

namespace Service.Discount;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Coupon> Coupons { get; set; } = default!;
}