using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Features.Status.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Status.ListarStatus;

public record StatusDto(int Id, string StatusSet);
public record ListarStatusQuery() : IRequest<IResult>;

public class ListarStatusHandler : IRequestHandler<ListarStatusQuery, IResult>
{
    private readonly AppDbContext _context;
    public ListarStatusHandler(AppDbContext context) => _context = context;

    public async Task<IResult> Handle(ListarStatusQuery request, CancellationToken ct)
    {
        var status = await _context.StatusItens
            .AsNoTracking()
            .Select(s => new StatusDto(
                s.Id, 
                s.Status 
            ))
            .ToListAsync(ct);

        return Results.Ok(status);
    }
}