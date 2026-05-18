using MediatR;
using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Security;

namespace API_AchadosEPerdidos.Features.Users.Login;

public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Nome, string Token);
public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse?>;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse?>
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public LoginHandler(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == command.Request.Email, ct);

        if (user == null || !PasswordHasher.ValidarSenha(command.Request.Senha, user.Senha))
        {
            return null;
        }

        if (!user.Confirmado)
        {
            throw new Exception("Conta ainda não confirmada. Verifique seu e-mail no Mailtrap.");
        }

        var tokenReal = _tokenService.GerarToken(user);

        return new LoginResponse(user.Nome, tokenReal);
    }
}