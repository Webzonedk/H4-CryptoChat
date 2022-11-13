using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;

namespace Server.Hubs;

public class ChatHub : Hub
{
    //This is basicly the server
    public Task SendMessage(string user, string message)
    {
        return Clients.All.SendAsync(method: "ReceiveMessage",user,message);
    }

    public Task SendPublicKey(string publicKeyString)
    {
        return Clients.Caller.SendAsync(method: "ReceivePublicKey", publicKeyString);
    } 
        public Task SendKey(string keyString)
    {
        return Clients.Caller.SendAsync(method: "ReceiveKey", keyString);
    } 

    public Task SendIV(string ivString)
    {
        return Clients.Caller.SendAsync(method: "ReceiveIV", ivString);
    }
}
