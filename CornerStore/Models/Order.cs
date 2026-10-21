
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
    [Required]
    public int CashierId { get; set; }
    public Cashier Cashier { get; set; }
    public List<OrderProduct> OrderProducts { get; set; }
    public decimal? Total
    {
        get
        {
            return OrderProducts.Sum(op => op.Quantity * op.Product?.Price);
        }
    }
    public DateTime? PaidOnDate { get; set; }
}