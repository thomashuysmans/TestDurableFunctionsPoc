using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace TestDurableFunctions
{
    public static class WorkflowOrchestrator
    {
        [FunctionName("Function1_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Good morning {name}!";
        }

        [FunctionName("Function1_Hello_In_Spanish")]
        public static string SayHelloInSpanish([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello in Spanish to {name}.");
            return $"Buenos dias {name}!";
        }

        [FunctionName("Function1_Hello_In_Danish")]
        public static string SayHelloInDanish([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello in Danish to {name}.");
            return $"Godmorgen {name}!";
        }
        
        [FunctionName("ExecuteWorkflow")]
        public static async Task<List<string>> ExecuteWorkflow([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            
            // We can get input parameters by using the context object. 
            var workflowId = context.GetInput<int>();
            var workflowManager = new WorkflowManager();
            var workflow = workflowManager.GetWorkflowByKey(workflowId);

            foreach (var action in workflow.Actions)
            {
                outputs.Add(await context.CallActivityAsync<string>(action.WorkflowActionName, "Tokyo"));
            }

            return outputs;
        }

        [FunctionName("WorkflowOrchestrator_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
            HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var workflowId = 1;
            var instanceId = await starter.StartNewAsync("ExecuteWorkflow", null, workflowId);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}