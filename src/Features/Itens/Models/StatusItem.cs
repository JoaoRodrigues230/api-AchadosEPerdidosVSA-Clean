using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Itens.Models;

[Table("status_item")]
public class StatusItem
{
    [Key] [Column("status_id")]
    public int Id { get; set; }

    [Column("status_set")]
    public string Status { get; set; } = null!;
}