using MediatR;
using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;

namespace API_AchadosEPerdidos.Features.Users.ConfirmUser;

public record ConfirmUserCommand(Guid Token) : IRequest<bool>;
public class ConfirmUserHandler : IRequestHandler<ConfirmUserCommand, bool>
{
    private readonly AppDbContext _context;

    public ConfirmUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ConfirmUserCommand request, CancellationToken ct)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.TokenConfirmacao == request.Token, ct);

        if (usuario == null) return false;

        usuario.Confirmado = true;

        usuario.TokenConfirmacao = Guid.Empty;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}