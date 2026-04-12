namespace API_AchadosEPerdidos.Features.Users.Login;

public record LoginRequest(string Email, string Senha);
public record LoginResponse(Guid Id, string Nome, string Email, int AcessoId);