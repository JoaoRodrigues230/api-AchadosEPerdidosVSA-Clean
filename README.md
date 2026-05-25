# Achados e Perdidos Backend build passing version 1.0.0

* Conta para testes:
  * Email: teste@unisantos.br
  * Senha: teste123

Uma arquitetura orientada a fatias verticais (*Vertical Slice Architecture) que integra serviços síncronos HTTP (REST) com um servidor de notificações persistentes via **Sockets TCP* (Port 11000) para gerenciamento de sessões e eventos em tempo real.

---

## Stack Tecnológica

* *Backend:* .NET Core SDK >= 8.0 / MediatR
* *Database:* PostgreSQL >= 13 (Instância já populada)
* *Real-time:* TCP Sockets (TcpListener / TcpClient)
* *Teste-Stress:* K6 (winget install grafana.k6 OU https://github.com/grafana/k6/releases/download/v0.51.0/k6-v0.51.0-amd64.msi&authuser=2)

---

## Inicialização do Ambiente

### 1. Servidor Backend (.NET)

Navegue até o diretório raiz do projeto backend e execute os comandos de restore e inicialização:

bash
dotnet restore
dotnet run

O console confirmará a subida de ambos os protocolos na mesma aplicação:

HTTP Gateway: http://localhost:5000

TCP Socket Listener: localhost:11000


## Fluxo de Execução & Teste (Sessão Concorrente)
O sistema implementa uma trava de segurança distribuída para evitar acessos simultâneos na mesma conta. Para demonstrar o funcionamento para a banca:

### Passo 1: Login Inicial (Navegador)
Acesse a aplicação pelo navegador e efetue o login com o usuário padrão.

O Front-end estabelecerá uma conexão persistente automática com o AchadosSocketServer na porta 11000 enviando o frame AUTH:UUID_DO_USUARIO.

### Passo 2: Login Concorrente (Swagger)
Abra uma janela anônima e force um novo login com a mesma conta utilizando a interface do Swagger (http://localhost:5000/swagger).

O LoginHandler processará a nova requisição via HTTP POST.

### Passo 3: Interceptação e Resposta (Real-time)
O barramento do MediatR disparará o método de broadcast síncrono no servidor de Sockets.

O backend localizará o canal de rede do Navegador no dicionário de memória (ClientesOnline).

O seguinte alerta será injetado no socket ativo do cliente antigo:

PlainText
ALERTA_SEGURANCA:Novo login detectado em outro dispositivo!

Resultado: O Front-end do Navegador interceptará o cabeçalho ALERTA_SEGURANCA e disparará a rotina de encerramento de sessão na interface gráfica.

## Alternativa de Debug (Opcional)
Caso queira isolar o comportamento do Socket sem a interface do React, o comando de autenticação pode ser simulado via terminal:

Bash
nc localhost 11000
AUTH:42840a9e-1f55-4cb0-a4b1-5390bf88805c

ou direto no PowerShell:

Bash
$client = New-Object System.Net.Sockets.TcpClient("localhost", 11000)
$stream = $client.GetStream()
$byteBuffer = [System.Text.Encoding]::UTF8.GetBytes("AUTH:42840a9e-1f55-4cb0-a4b1-5390bf88805c")                                                 $stream.Write($byteBuffer, 0, $byteBuffer.Length)
