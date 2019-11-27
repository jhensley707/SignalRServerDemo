using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class BackgroundTaskHub : Hub<ILongRunningTask>
    {
        private readonly ILogger<BackgroundTaskHub> _logger;
        private readonly IHubContext<BackgroundTaskHub, ILongRunningTask> _hubContext;

        public IBackgroundTaskQueue Queue { get; }

        public BackgroundTaskHub(ILogger<BackgroundTaskHub> logger, IBackgroundTaskQueue queue,
            IHubContext<BackgroundTaskHub, ILongRunningTask> hubContext)
        {
            _logger = logger;
            Queue = queue;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            // By creating a named group, connections on multiple hubs can be messaged together
            // By creating a unique name per connection and passing that group name into a 
            // background task, we should be able to message the original caller outside
            // this hub instance
            _logger.LogInformation("Adding connection id {0} as group name", Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Removing connection id {0} as group name", Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// The long running process on the server invokes this method
        /// to return the result once it is complete
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task ReturnTaskResult(TaskResult result)
        {
            await Clients.Caller.ReturnTaskResult(result);
        }

        /// <summary>
        /// The long running process on the server invokes this
        /// method to update the client.
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task ShowProgress(int progress)
        {
            await Clients.Caller.ShowProgress(progress);
        }

        /// <summary>
        /// The client calls this method on the server to begin the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task SubmitTaskRequest(TaskRequest request)
        {
            string groupId = Context.ConnectionId;
            Queue.QueueBackgroundWorkItem(async token =>
            {
                var rand = new Random();
                var guid = Guid.NewGuid().ToString();

                for (int delayLoop = 0; delayLoop <= 100; delayLoop += 10)
                {
                    try
                    {
                        _logger.LogInformation($"Delay loop {delayLoop} for {groupId}");
                        await _hubContext.Clients.Group(groupId).ShowProgress(delayLoop);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred sending progress. Error: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 5)), token);
                }
                await _hubContext.Clients.Group(groupId).ReturnTaskResult(new TaskResult { Name = request.Name, TaskId = guid });

                _logger.LogInformation($"Queued Background Task {guid} is complete.");
            });

            return Task.CompletedTask;
        }
    }
}
