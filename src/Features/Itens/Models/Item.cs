using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_AchadosEPerdidos.Shared.Infrastructure.Storage;
using API_AchadosEPerdidos.Features.Local.Models;
using API_AchadosEPerdidos.Features.Itens.Models;

namespace API_AchadosEPerdidos.Features.Itens.Models;

[Table("item")]
public class Item
{
    [Key] 
    [Column("item_id")] 
    public int Id { get; set; }

    [Column("item_nome")] 
    public string Nome { get; set; } = null!;

    [Column("item_dt_achado")] 
    public DateTime DataAchado { get; set; }

    [Column("item_loca_id")] 
    public int LocalId { get; set; }

    [Column("item_categ_id")] 
    public int CategoriaId { get; set; }

    [Column("item_descricao")] 
    public string? Descricao { get; set; }

    [Column("item_status_id")] 
    public int StatusId { get; set; }

    [Column("item_storage_link")] 
    public string? StorageLink { get; set; }

    [Column("item_info_add")] 
    public string? InfoAdd { get; set; }

    [ForeignKey(nameof(LocalId))]
    public virtual API_AchadosEPerdidos.Features.Local.Models.Local Local { get; set; } = null!;

    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria Categoria { get; set; } = null!;

    [ForeignKey(nameof(StatusId))]
    public virtual StatusItem StatusItem { get; set; } = null!;

    // Propriedade de navegação do EF Core (1:N)
    public virtual ICollection<ItemImage> Imagens { get; set; } = new List<ItemImage>();
}