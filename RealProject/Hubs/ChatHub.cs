using Microsoft.AspNetCore.SignalR;

namespace RealProject.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage");
            //await Clients.Caller.SendAsync("ReceiveMessage", m);
            //await Clients.Others.SendAsync("ReceiveMessage", m);
        }
    }
}
