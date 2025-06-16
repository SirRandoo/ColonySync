using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ColonySync.Plugins.Twitch;

public class TwitchEventSubClient(ILogger logger)
{
    private readonly object _lock = new();
    private CancellationTokenSource? _cancellationTokenSource;

    private string _currentUrl = "wss://eventsub.wss.twitch.tv/ws";
    private int _reconnectAttempts;
    private ClientWebSocket? _webSocket;

    public async Task ConnectAsync()
    {
        await ConnectInternalAsync(_currentUrl);
    }

    private async Task ConnectInternalAsync(string url)
    {
        _webSocket = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            logger.LogInformation(message: "Connecting to {0}", url);
            await _webSocket.ConnectAsync(new Uri(url), _cancellationTokenSource.Token);
            logger.LogInformation("WebSocket connected.");

            _reconnectAttempts = 0; // Reset on success

            _ = Task.Run(() => ReceiveLoopAsync(_cancellationTokenSource.Token));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Failed to connect");
            await AttemptReconnectWithBackoff();
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket!.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    logger.LogWarning("WebSocket closed by server.");
                    break;
                }

                string jsonString = Encoding.UTF8.GetString(buffer, index: 0, result.Count);
                HandleMessage(jsonString);
            }
        }
        catch (OperationCanceledException) {}
        catch (WebSocketException ex)
        {
            logger.LogError(ex, message: "WebSocket error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unknown error");
        }

        await CleanupAndReconnect();
    }

    private async Task CleanupAndReconnect()
    {
        await DisconnectAsync();
        await AttemptReconnectWithBackoff();
    }

    private async Task AttemptReconnectWithBackoff()
    {
        _reconnectAttempts++;
        int delay = Math.Min(val1: 30000, 1000 * (int)Math.Pow(x: 2, _reconnectAttempts)); // max 30s

        logger.LogInformation(message: "Attempting reconnect in {0} seconds...", (delay / 1000).ToString("N"));
        await Task.Delay(delay);

        await ConnectInternalAsync(_currentUrl);
    }

    private void HandleMessage(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty(propertyName: "metadata", out var metadata)) return;

        string? messageType = metadata.GetProperty("message_type").GetString();

        switch (messageType)
        {
            case "session_welcome":
                string? sessionId = root.GetProperty("payload").GetProperty("session").GetProperty("id").GetString();
                logger.LogInformation(message: "Session established. ID: {0}", sessionId);
                break;

            case "notification":
                HandleNotification(root);
                break;

            case "session_reconnect":
                string? reconnectUrl = root.GetProperty("payload").GetProperty("session").GetProperty("reconnect_url").GetString();
                logger.LogWarning(message: "Reconnect requested. New URL: {0}", reconnectUrl);

                lock (_lock)
                {
                    _currentUrl = reconnectUrl!;
                }

                // force reconnection
                _ = CleanupAndReconnect();
                break;

            case "session_keepalive":
                logger.LogDebug("Keepalive received.");
                break;

            default:
                logger.LogWarning(message: "Received unhandled message type: {0}", messageType);
                break;
        }
    }

    private void HandleNotification(JsonElement root)
    {
        string? subscriptionType = root.GetProperty("payload").GetProperty("subscription").GetProperty("type").GetString();
        var eventData = root.GetProperty("payload").GetProperty("event");

        logger.LogDebug(message: "Notification: {0}", subscriptionType);

        switch (subscriptionType)
        {
            case "stream.online":
                string? broadcasterId = eventData.GetProperty("broadcaster_user_id").GetString();
                logger.LogInformation(message: "Stream is online for {0}", broadcasterId);
                break;

            // Extend this as needed for more types
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket != null)
        {
            try
            {
                if (_webSocket.State == WebSocketState.Open)
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: "Client disconnect", CancellationToken.None);
            }
            catch {}

            _webSocket.Dispose();
            _webSocket = null;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
