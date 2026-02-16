using System.Net.WebSockets;

namespace ETL.WebSocket;

public class WebSocketManager : IHostedService
{
    public ClientWebSocket Socket { get; } = new();

    public async Task StartAsync(CancellationToken ct)
    {
        await Socket.ConnectAsync(new Uri("wss://panel.bineshafzar.ir/api/WebSocket/ETL/Get"), ct); // use env var 
    }

    public Task StopAsync(CancellationToken ct)
        => Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", ct);
}

