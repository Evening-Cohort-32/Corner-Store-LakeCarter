using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models.DTO;

public class OrderProductDTO
{
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    public ProductDTO Product { get; set; }
    [Required]
    public int OrderId { get; set; }
    [Required]
    public int Quantity { get; set; }
}