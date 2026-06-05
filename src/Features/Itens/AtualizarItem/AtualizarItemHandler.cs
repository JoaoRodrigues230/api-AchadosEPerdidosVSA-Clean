using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace API_AchadosEPerdidos.Features.Itens.AtualizarItem;

public record AtualizarItemRequest(string? Nome, string? Descricao, int? LocalId, int? StatusId);
public record AtualizarItemCommand(int Id, AtualizarItemRequest Request) : IRequest<IResult>;

public class AtualizarItemHandler : IRequestHandler<AtualizarItemCommand, IResult>
{
    private readonly AppDbContext _context;

    public AtualizarItemHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(AtualizarItemCommand command, CancellationToken ct)
    {
        var item = await _context.Itens.FindAsync(new object[] { command.Id }, ct);

        if (item == null)
            return Results.NotFound(new { mensagem = "Item não encontrado." });

        item.Nome = command.Request.Nome ?? item.Nome;
        item.Descricao = command.Request.Descricao ?? item.Descricao;
        item.LocalId = command.Request.LocalId ?? item.LocalId;
        item.StatusId = command.Request.StatusId ?? item.StatusId;

        await _context.SaveChangesAsync(ct);

        return Results.Ok(new { mensagem = "Item atualizado com sucesso!", itemId = item.Id });
    }
}