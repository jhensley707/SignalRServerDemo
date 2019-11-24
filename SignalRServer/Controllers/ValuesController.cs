using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SignalRServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<ProgressHub, IProgress> _progressHub;

        public IBackgroundTaskQueue Queue { get; }

        public ValuesController(IBackgroundTaskQueue queue,
            ILogger<ValuesController> logger,
            IHubContext<ProgressHub, IProgress> progressHub,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            Queue = queue;
            _progressHub = progressHub;
            _serviceScopeFactory = serviceScopeFactory;

        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // Get for this call should start a long, periodic process which will
            // notify progress.

            Queue.QueueBackgroundWorkItem(async token =>
            {
                var guid = Guid.NewGuid().ToString();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    //var db = scopedServices.GetRequiredService<AppDbContext>();

                    for (int delayLoop = 1; delayLoop < 4; delayLoop++)
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
                            await _progressHub.Clients.All.ShowProgress(delayLoop);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "An error occurred writing to the " +
                                "database. Error: {Message}", ex.Message);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(5), token);
                    }
                }

                _logger.LogInformation(
                    "Queued Background Task {Guid} is complete. 3/3", guid);
            });

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
