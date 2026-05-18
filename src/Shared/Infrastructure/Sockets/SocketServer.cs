using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Sockets;

public class NexusSocketServer : BackgroundService
{
    private readonly TcpListener _listener;
    private const int Porta = 11000; //porta que o client vai conectar

    public NexusSocketServer()
    {
        _listener = new TcpListener(IPAddress.Any, Porta);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener.Start();
        Console.WriteLine($"teste de Servidor de Sockets TCP Rodando na porta {Porta} ");

        //aceita conexão enquanto a api estiver rodando
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //aguarda o front conectar
                var client = await _listener.AcceptTcpClientAsync(stoppingToken);

                //evitando bloqueio de thread, jogando client em uma task a parte
                _ = Task.Run(() => TratarClienteAsync(client, stoppingToken), stoppingToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                Console.WriteLine($"erro no accept do Socket: {ex.Message}");
            }
        }

        _listener.Stop();
    }

    private async Task TratarClienteAsync(TcpClient client, CancellationToken ct)
    {
        Console.WriteLine("Cliente conectado via Socket TCP!");
        using var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            while (!ct.IsCancellationRequested)
            {
                int bytesLidos = await stream.ReadAsync(buffer, 0, buffer.Length, ct);

                //se os bytes que o client enviou forem 0 ele desconectou
                if (bytesLidos == 0) break;

                string mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, bytesLidos).Trim();
                Console.WriteLine($"Socket Recebeu: {mensagemRecebida}");

                string resposta = $"Processado: {mensagemRecebida}\n";
                byte[] bytesResposta = Encoding.UTF8.GetBytes(resposta);

                //devolutiva
                await stream.WriteAsync(bytesResposta, 0, bytesResposta.Length, ct);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"erro na comunicação do Socket: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Cliente deslogou do Socket TCP");
        }
    }
}