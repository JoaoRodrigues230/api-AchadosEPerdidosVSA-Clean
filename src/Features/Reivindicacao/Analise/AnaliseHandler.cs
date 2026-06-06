using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Features.Reivindicacoes.Models;
using API_AchadosEPerdidos.Features.Itens.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Reivindicacao.Analise;

public record AnalisarSolicitacaoRequest(Guid AdminId, bool Aprovado, string? MotivoReprovacao);
public record AnalisarSolicitacaoCommand(int ReivindicacaoId, AnalisarSolicitacaoRequest Request) : IRequest<IResult>;

public class AnalisarSolicitacaoHandler : IRequestHandler<AnalisarSolicitacaoCommand, IResult>
{
    private readonly AppDbContext _context;
    public AnalisarSolicitacaoHandler(AppDbContext context) => _context = context;

    public async Task<IResult> Handle(AnalisarSolicitacaoCommand command, CancellationToken ct)
    {
        var reivindicacao = await _context.Reivindicacoes
            .FirstOrDefaultAsync(r => r.Id == command.ReivindicacaoId, ct);

        if (reivindicacao == null)
            return Results.NotFound(new { mensagem = "Solicitação de reivindicação não encontrada." });

        if (reivindicacao.StatusId != 7) 
            return Results.BadRequest(new { mensagem = "Esta solicitação já foi analisada anteriormente." });

        var item = await _context.Itens.FirstOrDefaultAsync(i => i.Id == reivindicacao.ItemId, ct);
        if (item == null)
            return Results.NotFound(new { mensagem = "Item vinculado a esta solicitação sumiu do banco." });

        reivindicacao.UsuarioAnaliseId = command.Request.AdminId;
        reivindicacao.DataAnalise = DateTime.UtcNow;

        if (command.Request.Aprovado)
        {
            reivindicacao.StatusId = 5;
            item.StatusId = 3;         
        }
        else
        {
            reivindicacao.StatusId = 6; 
            reivindicacao.MotivoReprovacao = command.Request.MotivoReprovacao ?? "Provas insuficientes.";
            
            item.StatusId = 1;     
        }

        await _context.SaveChangesAsync(ct);

        return Results.Ok(new { 
            mensagem = command.Request.Aprovado ? "Solicitação aprovada com sucesso!" : "Solicitação reprovada e item liberado.",
            itemStatusId = item.StatusId,
            reivindicacaoStatusId = reivindicacao.StatusId
        });
    }
}