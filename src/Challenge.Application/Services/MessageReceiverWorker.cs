using Challenge.Domain.Bus;
using Challenge.Domain.Interfaces;
using Challenge.Infra.Client;
using Challenge.Infra.CrossCutting.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Challenge.Application.Services;
public class MessageReceiverWorker : BackgroundService
{
    private readonly ILogger<MessageReceiverWorker> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly INewsCache _newsCache;
    public bool isListening = false;

    private readonly IServiceScopeFactory _serviceProvider;
    private readonly ConcurrentQueue<string> _queueIds = null;
    private readonly HackNewsClient _client;

    private readonly Thread _threadListening, _threadSending = null;

    public MessageReceiverWorker(ILogger<MessageReceiverWorker> logger, INewsCache newsCache, IServiceScopeFactory provider, HackNewsClient client)
    {
        _newsCache = newsCache;
        _client = client;

        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();

        _serviceProvider = provider;
        _queueIds = new ConcurrentQueue<string>();

        _threadListening = new Thread(() => ListenToStreamAsync(_cancellationTokenSource.Token));
        _threadListening.Name = nameof(_threadListening);

        _threadSending = new Thread(() => SendMessageToHubAsync(_cancellationTokenSource.Token));
        _threadSending.Name = nameof(_threadListening);
    }

    public async override Task StartAsync(CancellationToken stoppingToken)
    {
        _threadListening.Start();
        _threadSending.Start();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private async Task ListenToStreamAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // header required for Firebase Streaming
                _client.HttpClient.DefaultRequestHeaders.Add("Accept", "text/event-stream");

                using (var request = new HttpRequestMessage(HttpMethod.Get, _client.GetUrlMaxItem()))
                using (var response = await _client.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, stoppingToken))
                {
                    response.EnsureSuccessStatusCode();

                    _client.HttpClient.DefaultRequestHeaders.Remove("Accept");

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        Console.WriteLine("<color=green>Connected! Waiting for real-time updates...</color>");

                        isListening = true;

                        while (!reader.EndOfStream)
                        {
                            var line = await reader.ReadLineAsync();

                            if (string.IsNullOrWhiteSpace(line)) continue;

                            if (line.StartsWith("data: "))
                            {
                                string jsonPayload = line.Substring(6).Trim();

                                if (jsonPayload == "null" || string.IsNullOrEmpty(jsonPayload)) continue;

                                var message = JsonSerializer.Deserialize<MessageReceivedJson>(jsonPayload);

                                _queueIds.Enqueue(message.data.ToString());
                            }
                        }

                        isListening = false;
                    }
                }
            }
        }
        catch (TaskCanceledException)
        {
            isListening = false;
            // Expected when stopping play mode
        }
        catch (Exception ex)
        {
            isListening = false;
            Console.WriteLine($"Stream Error: {ex.Message}");
        }
    }


    private async Task SendMessageToHubAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queueIds.TryDequeue(out string id))
            {
                try
                {
                    var store = await _client.GetStoryByIDAsync(id);
                    var result = await _newsCache.UpsertNewsAsync(store);

                    if (result.IsSuccess)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<BrokerHub>>();

                            await hubContext.Clients.Groups("HackNewsMessage").SendAsync("ReceiveMessageHN", store);
                        }
                    }
                    else
                    {
                        _queueIds.Enqueue(id);
                    }
                }
                catch (Exception ex)
                {
                    _queueIds.Enqueue(id);
                    _logger.LogError(ex, ex.Message);
                }
            }
            await Task.Delay(TimeSpan.FromMilliseconds(20));
        }
    }
}