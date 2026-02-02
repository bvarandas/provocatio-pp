using Microsoft.AspNetCore.SignalR;
namespace Challenge.Infra.CrossCutting.Hubs;

public class BrokerHub : Hub
{
    public Task ConnectToMessageBroker()
    {
        Groups.AddToGroupAsync(Context.ConnectionId, "HackNewsMessage");

        return Task.CompletedTask;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    }
}
