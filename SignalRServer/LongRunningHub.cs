using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class LongRunningHub : Hub<ILongRunningTask>
    {
        private readonly ILogger<LongRunningHub> _logger;

        public LongRunningHub(ILogger<LongRunningHub> logger)
        {
            _logger = logger;
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
        public async Task SubmitTaskRequest(TaskRequest request)
        {
            var rand = new Random();
            var guid = Guid.NewGuid().ToString();

            for (int delayLoop = 0; delayLoop <= 100; delayLoop += 10)
            {
                try
                {
                    _logger.LogInformation($"Delay loop {delayLoop}");
                    await ShowProgress(delayLoop);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred sending progress. Error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 5)));
            }

            await ReturnTaskResult(new TaskResult { Name = request.Name, TaskId = guid });
            _logger.LogInformation(
                "LongRunning Task {Guid} is complete.", guid);
        }
    }
}
