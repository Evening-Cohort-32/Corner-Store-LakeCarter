

namespace CornerStore.Models.DTO;

public class OrderDTO
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public CashierDTO Cashier { get; set; }
    public List<OrderProductDTO> OrderProducts { get; set; }
    public decimal Total
    {
        get
        {
            return OrderProducts.Sum(op => op.Quantity * op.Product.Price);
        }
    }
    public DateTime? PaidOnDate { get; set; }
}