using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Security;
using API_AchadosEPerdidos.Features.Users.Models;


namespace API_AchadosEPerdidos.Features.Users.CreateUser;

public record CreateUserRequest(string Nome, string Email, string Senha, string Cpf, string Telefone, int AcessoId);
public record CreateUserCommand(CreateUserRequest Request) : IRequest<Guid>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly AppDbContext _context;
    public CreateUserHandler(AppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var dto = command.Request;
        var novoUsuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = PasswordHasher.HashSenha(dto.Senha),
            Cpf = dto.Cpf,
            Telefone = dto.Telefone,
            AcessoId = dto.AcessoId
        };

        var cpfLimpo = dto.Cpf.Replace(".", "").Replace("-", "").Trim();

        if (cpfLimpo.Length != 11)
        {
            throw new Exception("O CPF deve conter exatamente 11 dígitos.");
        }

        if (!dto.Email.EndsWith("@unisantos.br"))
        {
            throw new Exception("Apenas e-mails institucionais são permitidos.");
        }

        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync(ct);
        return novoUsuario.Id;
    }
}