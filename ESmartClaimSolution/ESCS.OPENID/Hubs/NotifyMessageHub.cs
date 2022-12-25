using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.OPENID.Hubs
{
    /// <summary>
    /// IHubContext<NotifyMessageHub>
    /// _notificationHubContext.Clients.All.SendAsync("ReceiveNotify", "OpenId", "Thanh test");
    /// Client nhận bằng phương thức ReceiveNotify
    /// </summary>
    public class NotifyMessageHub : Hub
    {
       
    }
}
