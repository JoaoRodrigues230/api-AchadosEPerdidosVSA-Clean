using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace API_AchadosEPerdidos.Features.Users.Models;

[Table("acesso_usuario")]
public class AcessoUsuario
{
    [Key]
    [Column("acess_id")]
    public int Id { get; set; }

    [Column("acess_type")]
    public string Descricao { get; set; } = string.Empty;
}