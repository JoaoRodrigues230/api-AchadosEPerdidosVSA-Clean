using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Features.Reivindicacoes.Models; 
using API_AchadosEPerdidos.Features.Itens.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Reivindicacao.Solicitacao;

public record SolicitarReivindicacaoRequest(Guid UsuarioId, string DescricaoDaProva);
public record SolicitarReivindicacaoCommand(int ItemId, SolicitarReivindicacaoRequest Request) : IRequest<IResult>;

public class SolicitarReivindicacaoHandler : IRequestHandler<SolicitarReivindicacaoCommand, IResult>
{
    private readonly AppDbContext _context;
    public SolicitarReivindicacaoHandler(AppDbContext context) => _context = context;

    public async Task<IResult> Handle(SolicitarReivindicacaoCommand command, CancellationToken ct)
    {
        var item = await _context.Itens.FirstOrDefaultAsync(i => i.Id == command.ItemId, ct);

        if (item == null) 
            return Results.NotFound(new { mensagem = "Item não encontrado." });
        
        if (item.StatusId != 1)
            return Results.BadRequest(new { message = "Este item não está disponível para reivindicação." });

        var novaReivindicacao = new API_AchadosEPerdidos.Features.Reivindicacoes.Models.Reivindicacao()
        {
            ItemId = item.Id,
            UsuarioId = command.Request.UsuarioId,
            Prova = command.Request.DescricaoDaProva,
            StatusId = 7,
            DataSolicitacao = DateTime.UtcNow
        };

        item.StatusId = 3;

        _context.Reivindicacoes.Add(novaReivindicacao);

        await _context.SaveChangesAsync(ct);
        
        return Results.Ok(new { 
            mensagem = "Solicitação gerada com sucesso! Aguarde a análise da secretaria.", 
            itemStatusId = item.StatusId 
        });
    }
}