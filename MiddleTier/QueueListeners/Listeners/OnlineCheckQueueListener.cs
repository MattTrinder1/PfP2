using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QueueListeners
{
    public class OnlineCheckQueueListener
    {
        [Function("OnlineCheckQueueListener")]
        public void Run([QueueTrigger("onlinecheckqueue")] string myQueueItem, FunctionContext context)
        {
            var log = context.GetLogger("OnlineCheckQueueListener");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
