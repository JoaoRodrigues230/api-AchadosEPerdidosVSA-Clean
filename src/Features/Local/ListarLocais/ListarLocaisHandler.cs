using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Locais.ListarLocais;

public record LocalDto(int Id, string Nome);
public record ListarLocaisQuery() : IRequest<IResult>;

public class ListarLocaisHandler : IRequestHandler<ListarLocaisQuery, IResult>
{
    private readonly AppDbContext _context;
    public ListarLocaisHandler(AppDbContext context) => _context = context;

    public async Task<IResult> Handle(ListarLocaisQuery request, CancellationToken ct)
    {
        var locais = await _context.Locais
            .AsNoTracking()
            .Select(l => new LocalDto(l.Id, l.Nome))
            .ToListAsync(ct);

        return Results.Ok(locais);
    }
}