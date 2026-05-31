using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Itens.Models;

[Table("item_image")]
public class ItemImage
{
    [Key] 
    [Column("image_id")] 
    public int Id { get; set; }

    [Column("image_item_id")] 
    public int ItemId { get; set; }

    [Column("image_url")] 
    public string Url { get; set; } = null!;

    [Column("is_main")] 
    public bool IsMain { get; set; }

    [Column("image_dt_criacao")] 
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; } = null!;
}