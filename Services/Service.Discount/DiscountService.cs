using Discount.Grpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Service.Discount;

public class DiscountService(DataContext dataContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        logger.LogInformation("Discount grpc is calling");
        
        var coupon = await dataContext.Coupons
            .FirstOrDefaultAsync(x => x.ProductName.Equals(request.ProductName));

        if (coupon is null) return new CouponModel();
        
        return new CouponModel
        {
            Id = coupon.Id,
            Amount = coupon.Amount,
            ProductName = coupon.ProductName
        };
    }
    
    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var entity = new Coupon
        {
            ProductName = request.ProductName,
            Amount = request.Amount
        };

        await dataContext.Coupons.AddAsync(entity);
        await dataContext.SaveChangesAsync();
        
        return new CouponModel
        {
            Id = entity.Id,
            Amount = entity.Amount,
            ProductName = entity.ProductName
        };
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var entity = await dataContext
            .Coupons
            .FirstOrDefaultAsync(x => x.Id.Equals(request.Coupon.Id));

        if (entity is null) return new CouponModel();

        entity.ProductName = request.Coupon.ProductName;
        entity.Amount = request.Coupon.Amount;

        await dataContext.SaveChangesAsync();
        
        return new CouponModel
        {
            Id = entity.Id,
            Amount = entity.Amount,
            ProductName = entity.ProductName
        };
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var entity = await dataContext
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName.Equals(request.ProductName));

        if (entity is null) return new DeleteDiscountResponse
        {
            Success = false
        };

        dataContext.Remove(entity);
        await dataContext.SaveChangesAsync();

        return new DeleteDiscountResponse
        {
            Success = true
        };
    }
}