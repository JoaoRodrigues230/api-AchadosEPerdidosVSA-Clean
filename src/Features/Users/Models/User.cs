using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Users.Models;

[Table("usuario")]
public class Usuario
{
    [Key]
    [Column("usua_id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("usua_nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("usua_email")]
    public string Email { get; set; } = string.Empty;

    [Column("usua_senha")]
    public string Senha { get; set; } = string.Empty;

    [Column("usua_acesso")]
    public int AcessoId { get; set; }

    [Column("usua_cpf")]
    public string Cpf { get; set; } = string.Empty;

    [Column("usua_telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("usua_datacriacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [ForeignKey("AcessoId")]
    public virtual AcessoUsuario Acesso { get; set; } = null!;
}