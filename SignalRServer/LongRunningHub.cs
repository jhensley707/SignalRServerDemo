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

        public IBackgroundTaskQueue Queue { get; }

        public LongRunningHub(ILogger<LongRunningHub> logger, IBackgroundTaskQueue queue)
        {
            _logger = logger;
            Queue = queue;
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
            var guid = Guid.NewGuid().ToString();

            for (int delayLoop = 0; delayLoop <= 100; delayLoop += 10)
            {
                try
                {
                    _logger.LogInformation($"Delay loop {delayLoop}");
                    //db.Messages.Add(
                    //    new Message()
                    //    {
                    //        Text = $"Queued Background Task {guid} has " +
                    //            $"written a step. {delayLoop}/3"
                    //    });
                    //await db.SaveChangesAsync();
                    await ShowProgress(delayLoop);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "An error occurred writing to the " +
                        "database. Error: {Message}", ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            await ReturnTaskResult(new TaskResult { Name = request.Name, TaskId = guid });
            _logger.LogInformation(
                "LongRunning Task {Guid} is complete. 3/3", guid);
        }
    }
}
