using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace phase_2_back_end.CentralHub
{
    public class SignalrHub : Hub
    {
        public static class UserHandler
        {
            public static HashSet<string> ConnectedIds = new HashSet<string>();
            public static int LiveNumber;
        }

        public async Task BroadcastNumber(int number)
        {
            UserHandler.LiveNumber = number;
            await Clients.All.SendAsync("UpdateNumber", number);
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            int usersCount = UserHandler.ConnectedIds.Count();
            Clients.All.SendAsync("ShowUserCounts", usersCount);
            Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, "has connected!");
            Clients.Caller.SendAsync("UpdateNumber", UserHandler.LiveNumber);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            int usersCount = UserHandler.ConnectedIds.Count();
            Clients.All.SendAsync("ShowUserCounts", usersCount);
            Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, "has left.");
            return base.OnDisconnectedAsync(ex);
        }
    }
}
