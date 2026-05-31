using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Itens.AtualizarItem;

public record AtualizarItemCommand(
    int Id,
    string Nome,
    DateTime DataAchado,
    int LocalId,
    int CategoriaId,
    int StatusId,
    string? Descricao,
    string? InfoAdd
) : IRequest<IResult>;

public class AtualizarItemHandler : IRequestHandler<AtualizarItemCommand, IResult>
{
    private readonly AppDbContext _context;

    public AtualizarItemHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(AtualizarItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Itens
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return Results.NotFound(new { mensagem = "Item não encontrado para atualização." });
        }

        item.Nome = request.Nome;
        item.DataAchado = request.DataAchado;
        item.LocalId = request.LocalId;
        item.CategoriaId = request.CategoriaId;
        item.StatusId = request.StatusId;
        item.Descricao = request.Descricao;
        item.InfoAdd = request.InfoAdd;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { mensagem = "Item atualizado com sucesso!", itemId = item.Id });
        }
        catch (Exception ex)
        {
            return Results.Json(new { 
                erro = "Falha ao tentar atualizar o item.", 
                detalhes = ex.InnerException?.Message ?? ex.Message 
            }, statusCode: 500);
        }
    }
}