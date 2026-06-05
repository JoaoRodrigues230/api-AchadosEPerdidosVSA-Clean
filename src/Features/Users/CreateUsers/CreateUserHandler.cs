using MediatR;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Security;
using API_AchadosEPerdidos.Features.Users.Models;
using API_AchadosEPerdidos.Shared.Infrastructure.Email;


namespace API_AchadosEPerdidos.Features.Users.CreateUser;

public record CreateUserRequest(string Nome, string Email, string Senha, string Cpf, string Telefone, int AcessoId);
public record CreateUserCommand(CreateUserRequest Request) : IRequest<Guid>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;
    public CreateUserHandler(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var dto = command.Request;
        var token = Guid.NewGuid();

        var novoUsuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = PasswordHasher.HashSenha(dto.Senha),
            Cpf = dto.Cpf,
            Telefone = dto.Telefone,
            AcessoId = dto.AcessoId,
            Confirmado = false,
            TokenConfirmacao = token
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

        var baseUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL") ?? "http://localhost:5000";

        var linkConfirmacao = $"{baseUrl}/usuario/confirm?token={token}";

        await _emailService.SendEmailAsync(
            novoUsuario.Email,
            "Bem-vindo ao Achados e Perdidos - Confirme seu E-mail",
            $"<h1>Olá, {novoUsuario.Nome}!</h1><p>Clique <a href='{linkConfirmacao}'>aqui</a> para validar sua conta.</p>"
        );

        return novoUsuario.Id;
    }
}