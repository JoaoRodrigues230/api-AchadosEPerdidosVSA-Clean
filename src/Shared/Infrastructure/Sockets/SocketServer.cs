using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Sockets;

public class AchadosSocketServer : BackgroundService
{
    private readonly TcpListener _listener;
    private const int Porta = 11000; //porta que o client vai conectar

    private static readonly Dictionary<Guid, NetworkStream> ClientesOnline = new();

    public AchadosSocketServer()
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
        Console.WriteLine("Client conectado via Socket TCP!");
        using var stream = client.GetStream();
        var buffer = new byte[1024];
        Guid? usuarioAutenticadoId = null;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                int bytesLidos = await stream.ReadAsync(buffer, 0, buffer.Length, ct);

                //se os bytes que o client enviou forem 0 ele desconectou
                if (bytesLidos == 0) break;

                string mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, bytesLidos);
                Console.WriteLine($"Socket Recebeu Raw: {mensagemRecebida.Trim()}");

                //lógica de tratamento de autenticação
                if (mensagemRecebida.StartsWith("AUTH:"))
                {
                    var idStr = mensagemRecebida.Replace("AUTH:", "")
                                                .Replace("\r", "")
                                                .Replace("\n", "")
                                                .Trim();

                    if (Guid.TryParse(idStr, out Guid userId))
                    {
                        usuarioAutenticadoId = userId;
                        
                        lock (ClientesOnline)
                        {
                            ClientesOnline[userId] = stream;
                        }
                        
                        Console.WriteLine($"usuário mapeado no Socket com ID: {userId}");

                        string respostaAuth = "STATUS:AUTENTICADO\n";
                        byte[] bytesAuth = Encoding.UTF8.GetBytes(respostaAuth);
                        await stream.WriteAsync(bytesAuth, 0, bytesAuth.Length, ct);
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"ID de usuário inválido recebido no Socket: {idStr}");
                    }
                }

                string resposta = $"Processado: {mensagemRecebida.Trim()}\n";
                byte[] bytesResposta = Encoding.UTF8.GetBytes(resposta);
                await stream.WriteAsync(bytesResposta, 0, bytesResposta.Length, ct);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"erro na comunicação do Socket: {ex.Message}");
        }
        finally
        {
            //em caso de desconexão, remove o usuário do dicionário
            if (usuarioAutenticadoId.HasValue)
            {
                lock (ClientesOnline)
                {
                    ClientesOnline.Remove(usuarioAutenticadoId.Value);
                }
                Console.WriteLine($"Usuário {usuarioAutenticadoId.Value} desconectou e foi removido do mapa.");
            }

            client.Close();
            Console.WriteLine("client deslogou do Socket TCP");
        }
    }

    public static async Task NotificarLoginDuplo(Guid userId)
    {
        NetworkStream? streamAnterior;

        lock (ClientesOnline)
        {
            ClientesOnline.TryGetValue(userId, out streamAnterior);
        }

        if (streamAnterior != null)
        {
            try
            {
                string msgAlerta = "ALERTA_SEGURANCA:Novo login detectado em outro dispositivo!\n";
                byte[] bytes = Encoding.UTF8.GetBytes(msgAlerta);
                await streamAnterior.WriteAsync(bytes, 0, bytes.Length);
                Console.WriteLine($"nova tentativa de login para o usuário {userId}");
            }
            catch
            {
                Console.WriteLine($"falha ao comunicar com a sessão antiga do usuário {userId}");
            }
        }
    }
}