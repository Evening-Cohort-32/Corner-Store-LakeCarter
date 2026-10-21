using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class OrderProduct
{
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Required]
    public int OrderId { get; set; }
    [Required]
    public int Quantity { get; set; }
}