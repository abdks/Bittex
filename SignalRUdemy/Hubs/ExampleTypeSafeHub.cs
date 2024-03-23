using Microsoft.AspNetCore.SignalR;

namespace SignalRUdemy.Hubs
{
    public class ExampleTypeSafeHub : Hub<IExampleTypeSafeHub>
    {
        public async Task BroadcastMessageToAllClient(string message)
        {
            await Clients.All.ReceiveMessageForAllClient(message);
        }
    }
}

