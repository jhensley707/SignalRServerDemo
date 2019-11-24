using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class ProgressHub : Hub<IProgress>
    {
        public async Task ShowProgress(int progress)
        {
            await Clients.Caller.ShowProgress(progress);
        }
    }
}
