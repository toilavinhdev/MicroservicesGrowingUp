using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Service.Ordering;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string UserName { get; set; } = default!;
    
    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    public string FullName { get; set; } = default!;
}