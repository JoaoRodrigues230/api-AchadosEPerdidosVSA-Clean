using MediatR;
using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Security;
using API_AchadosEPerdidos.Shared.Infrastructure.Sockets;
using System.Diagnostics;

namespace API_AchadosEPerdidos.Features.Users.Login;

public record LoginRequest(string Email, string Senha);
//FinOps PCG (implementar em pasta de testes futuramente)
public record LoginResponse(string Nome, string Token, object FinOps);
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
        var processoAtual = Process.GetCurrentProcess();
        var stopwatch = Stopwatch.StartNew();
        var tempoCpuInicial = processoAtual.TotalProcessorTime;

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

        //simulação de um algoritmo ineficiente para fins de teste de performance
        //o teste evidenciado de melhoria de performance foi feito comentando as linhas abaixo, 
        //o que resultou em uma redução significativa do tempo de resposta da API
        // long complexidadeDesnecessaria = 0;
        // for (int i = 0; i < 80_000_000; i++) 
        // {
        //     complexidadeDesnecessaria += i; 
        // }
        // _ = complexidadeDesnecessaria;

        var tokenReal = _tokenService.GerarToken(user);

        await AchadosSocketServer.NotificarLoginDuplo(user.Id);

        stopwatch.Stop();
        var tempoCpuFinal = processoAtual.TotalProcessorTime;

        var tempoDeCpuConsumido = (tempoCpuFinal - tempoCpuInicial).TotalMilliseconds;
        var tempoTotalPassado = stopwatch.Elapsed.TotalMilliseconds;

        //simulação baseada em uma instância AWS EC2 t3.medium de R$ 0,25/hora
        double usoCpuPorcentagem = Math.Round((tempoDeCpuConsumido / (tempoTotalPassado * Environment.ProcessorCount)) * 100, 2);
        if (usoCpuPorcentagem > 100) usoCpuPorcentagem = 100;

        double custoBasePorSegundo = 0.25 / 3600; 
        double fatorCarga = 1.0 + (usoCpuPorcentagem / 100.0); 
        double custoFinalReais = Math.Round(custoBasePorSegundo * (tempoTotalPassado / 1000) * fatorCarga, 6);

        var metricasFinOps = new
        {
            UsoMetrica = "FinOps (Cloud Financial Operations)",
            ConsumoHardware = new
            {
                UsoCPU = $"{usoCpuPorcentagem}%",
                TempoProcessamento = $"{Math.Round(tempoTotalPassado, 2)} ms"
            },
            ImpactoFinanceiro = new
            {
                Moeda = "BRL (R$)",
                CustoDestaOperacao = custoFinalReais,
                AnaliseComportamental = "Custo calculado sob demanda proporcional ao tempo de retenção da Thread e pico de CPU."
            }
        };

        return new LoginResponse(user.Nome, tokenReal, metricasFinOps);
    }
}