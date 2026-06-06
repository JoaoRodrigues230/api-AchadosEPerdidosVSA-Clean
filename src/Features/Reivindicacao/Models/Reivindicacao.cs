using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_AchadosEPerdidos.Features.Reivindicacoes.Models;

[Table("reivindicacoes")] 
public class Reivindicacao
{
    [Column("reiv_id")]
    public int Id { get; set; }

    [Column("reiv_item_id")]
    public int ItemId { get; set; }

    [Column("reiv_usua_id")]
    public Guid UsuarioId { get; set; }

    [Column("reiv_dt_solicitacao")]
    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

    [Column("reiv_prova")]
    public string Prova { get; set; } = string.Empty;

    [Column("reiv_status_id")]
    public int StatusId { get; set; }

    [Column("reiv_motivo_reprovacao")]
    public string? MotivoReprovacao { get; set; }

    [Column("reiv_usua_analise_id")]
    public Guid? UsuarioAnaliseId { get; set; }

    [Column("reiv_dt_analise")]
    public DateTime? DataAnalise { get; set; }
}