using Microsoft.AspNetCore.SignalR;
namespace API.Hubs
{
    public class ChatHub : Hub
    {
        //This is basicly the server
        public Task SendMessage(string message)
        {
            return Clients.All.SendAsync(method: "ReceiveMessage", message);
        }

    }
}
