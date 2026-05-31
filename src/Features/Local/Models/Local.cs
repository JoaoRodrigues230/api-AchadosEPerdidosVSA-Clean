using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Local.Models;

[Table("local")]
public class Local
{
    [Key] [Column("loca_id")]
    public int Id { get; set; }

    [Column("loca_nome")]
    public string Nome { get; set; } = null!;
}