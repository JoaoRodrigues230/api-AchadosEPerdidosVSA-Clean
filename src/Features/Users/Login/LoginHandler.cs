using MediatR;
using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Security;

namespace API_AchadosEPerdidos.Features.Users.Login;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse?>;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse?>
{
    private readonly AppDbContext _context;
    public LoginHandler(AppDbContext context) => _context = context;

    public async Task<LoginResponse?> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == command.Request.Email, ct);

        if (user == null) return null;

        var senhaValida = PasswordHasher.ValidarSenha(command.Request.Senha, user.Senha);

        if (!senhaValida) return null;

        return new LoginResponse(user.Id, user.Nome, user.Email, user.AcessoId);
    }
}