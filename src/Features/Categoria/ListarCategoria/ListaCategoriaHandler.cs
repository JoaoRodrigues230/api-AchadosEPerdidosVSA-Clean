using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Categorias.ListarCategorias;

public record CategoriaDto(int Id, string Nome);
public record ListarCategoriasQuery() : IRequest<IResult>;

public class ListarCategoriasHandler : IRequestHandler<ListarCategoriasQuery, IResult>
{
    private readonly AppDbContext _context;
    public ListarCategoriasHandler(AppDbContext context) => _context = context;

    public async Task<IResult> Handle(ListarCategoriasQuery request, CancellationToken ct)
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .Select(c => new CategoriaDto(c.Id, c.Nome))
            .ToListAsync(ct);

        return Results.Ok(categorias);
    }
}