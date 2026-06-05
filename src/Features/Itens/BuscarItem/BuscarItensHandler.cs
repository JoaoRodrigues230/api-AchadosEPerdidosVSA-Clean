using API_AchadosEPerdidos.Features.Itens.Models;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Itens.BuscarItensHandler;

public record ItemListaDto(
    int Id,
    string Nome,
    DateTime DataAchado,
    int CategoriaId,       
    string CategoriaNome,
    int LocalId,         
    string LocalNome,
    int StatusId,       
    string StatusNome,
    string? Descricao,
    string? FotoCapaUrl
);

public record BuscarItensQuery(
    string? TermoBusca,
    int? CategoriaId,
    int? LocalId,
    int? StatusId
) : IRequest<IResult>;

public class BuscarItensHandler : IRequestHandler<BuscarItensQuery, IResult>
{
    private readonly AppDbContext _context;

    public BuscarItensHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(BuscarItensQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Itens
            .Include(i => i.Categoria)
            .Include(i => i.Local)
            .Include(i => i.StatusItem)
            .Include(i => i.Imagens)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.ToLower();
            query = query.Where(i => i.Nome.ToLower().Contains(termo) || 
                                    (i.Descricao != null && i.Descricao.ToLower().Contains(termo)));
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(i => i.CategoriaId == request.CategoriaId.Value);
        }

        if (request.LocalId.HasValue)
        {
            query = query.Where(i => i.LocalId == request.LocalId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(i => i.StatusId == request.StatusId.Value);
        }

        var itens = await query
            .OrderByDescending(i => i.DataAchado)
            .Select(i => new ItemListaDto(
                i.Id,
                i.Nome,
                i.DataAchado,
                i.CategoriaId,     
                i.Categoria.Nome,
                i.LocalId,          
                i.Local.Nome,
                i.StatusId,          
                i.StatusItem.Status,
                i.Descricao,
                i.Imagens.FirstOrDefault(img => img.IsMain).Url ?? i.Imagens.FirstOrDefault().Url
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(itens);
    }
}