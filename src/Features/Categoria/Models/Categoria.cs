using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Categoria.Models;

[Table("categoria")]
public class Categoria
{
    [Key] [Column("categ_id")]
    public int Id { get; set; }

    [Column("categ_nome")]
    public string Nome { get; set; } = null!;
}