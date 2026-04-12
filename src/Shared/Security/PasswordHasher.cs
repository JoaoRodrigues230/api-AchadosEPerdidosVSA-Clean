namespace API_AchadosEPerdidos.Shared.Security;

public static class PasswordHasher
{
    public static string HashSenha(string senha)
        => BCrypt.Net.BCrypt.HashPassword(senha);

    //func pra verificar se a senha digitada no login bate com o hash do banco
    public static bool ValidarSenha(string senha, string hash)
        => BCrypt.Net.BCrypt.Verify(senha, hash);
}