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

public record ItemDetalheDto(
    int Id,
    string Nome,
    string? Descricao,
    DateTime DataAchado,
    int CategoriaId,
    string CategoriaNome,
    int LocalId,
    string LocalNome,
    int StatusId,
    string StatusNome,
    List<string> ImagensUrl
);

public record BuscarItemPorIdQuery(int Id) : IRequest<IResult>;

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

        if (request.CategoriaId.HasValue) query = query.Where(i => i.CategoriaId == request.CategoriaId.Value);
        if (request.LocalId.HasValue) query = query.Where(i => i.LocalId == request.LocalId.Value);
        if (request.StatusId.HasValue) query = query.Where(i => i.StatusId == request.StatusId.Value);

        var itens = await query
            .OrderByDescending(i => i.DataAchado)
            .Select(i => new ItemListaDto(
                i.Id,
                i.Nome,
                i.DataAchado,
                i.CategoriaId,     
                i.Categoria != null ? i.Categoria.Nome : "Sem Categoria",
                i.LocalId,          
                i.Local != null ? i.Local.Nome : "Sem Local",
                i.StatusId,          
                i.StatusItem != null ? i.StatusItem.Status : "Sem Status",
                i.Descricao,
                i.Imagens.FirstOrDefault(img => img.IsMain) != null 
                    ? i.Imagens.FirstOrDefault(img => img.IsMain)!.Url 
                    : (i.Imagens.FirstOrDefault() != null ? i.Imagens.FirstOrDefault()!.Url : "")
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(itens);
    }
}

public class BuscarItemPorIdHandler : IRequestHandler<BuscarItemPorIdQuery, IResult>
{
    private readonly AppDbContext _context;

    public BuscarItemPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(BuscarItemPorIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.Itens
            .Include(i => i.Categoria)
            .Include(i => i.Local)
            .Include(i => i.StatusItem)
            .Include(i => i.Imagens) 
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return Results.NotFound(new { mensagem = "Item não encontrado." });
        }

        var dto = new ItemDetalheDto(
            item.Id,
            item.Nome,
            item.Descricao,
            item.DataAchado,
            item.CategoriaId,
            item.Categoria != null ? item.Categoria.Nome : "Sem Categoria",
            item.LocalId,
            item.Local != null ? item.Local.Nome : "Sem Local",
            item.StatusId,
            item.StatusItem != null ? item.StatusItem.Status : "Sem Status",
            item.Imagens.Select(img => img.Url).ToList() 
        );

        return Results.Ok(dto);
    }
}