using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Hubs
{
    public class NotifyMessageHub: Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = this.Context.GetHttpContext();
            var tokenValue = httpContext.Request.Query["token"];
            return base.OnConnectedAsync();
        }
    }
}
