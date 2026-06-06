using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Reivindicacao.ObterPendentes;

public record SolicitacaoPendenteDto(
    int ReivindicacaoId,
    int ItemId,
    string ItemNome,
    Guid AlunoId,
    DateTime DataSolicitacao,
    string ProvaEscrita
);

public record ObterSolicitacoesPendentesQuery() : IRequest<IResult>;

public class ObterSolicitacoesPendentesHandler : IRequestHandler<ObterSolicitacoesPendentesQuery, IResult>
{
    private readonly AppDbContext _context;

    public ObterSolicitacoesPendentesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(ObterSolicitacoesPendentesQuery request, CancellationToken ct)
    {
        var solicitacoes = await _context.Reivindicacoes
            .AsNoTracking()
            .Where(r => r.StatusId == 7) 
            .OrderBy(r => r.DataSolicitacao) 
            .Select(r => new SolicitacaoPendenteDto(
                r.Id,
                r.ItemId,
                _context.Itens.FirstOrDefault(i => i.Id == r.ItemId) != null 
                    ? _context.Itens.FirstOrDefault(i => i.Id == r.ItemId)!.Nome 
                    : "Item Desconhecido",
                r.UsuarioId,
                r.DataSolicitacao,
                r.Prova
            ))
            .ToListAsync(ct);

        return Results.Ok(solicitacoes);
    }
}