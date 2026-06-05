using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Status.Models;

[Table("status_item")]
public class Status
{
    [Key] [Column("status_id")]
    public int Id { get; set; }

    [Column("status_set")]
    public string StatusSet { get; set; } = null!;
}