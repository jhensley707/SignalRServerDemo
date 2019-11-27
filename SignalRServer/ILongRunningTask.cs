using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer
{
    public interface ILongRunningTask : IProgress
    {
        Task SubmitTaskRequest(TaskRequest request);

        Task ReturnTaskResult(TaskResult result);
    }
}
