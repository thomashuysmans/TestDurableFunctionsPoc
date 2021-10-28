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
        
        // [FunctionName("Function1")]
        // public static async Task<List<string>> RunOrchestrator3(
        //     [OrchestrationTrigger] IDurableOrchestrationContext context)
        // {
        //     var outputs = new List<string>();
        //
        //     // Replace "hello" with the name of your Durable Activity Function.
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello_In_Danish", "Tokyo"));
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello_In_Spanish", "Seattle"));
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", "London"));
        //
        //     // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        //     return outputs;
        // }
        //
        //
        // [FunctionName("Function2")]
        // public static async Task<List<string>> RunOrchestrator2(
        //     [OrchestrationTrigger] IDurableOrchestrationContext context)
        // {
        //     var outputs = new List<string>();
        //
        //     // Replace "hello" with the name of your Durable Activity Function.
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello_In_Spanish", "Tokyo"));
        //     context.SetCustomStatus("Waiting for Godot..........");
        //     Thread.Sleep(10000);
        //
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", "Seattle"));
        //     outputs.Add(await context.CallActivityAsync<string>("Function1_Hello_In_Danish", "London"));
        //
        //     context.SetCustomStatus("Godot is here..........");
        //
        //     // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        //     return outputs;
        // }
        
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
        
        [FunctionName("WorkflowOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("WorkflowOrchestrator_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("WorkflowOrchestrator_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("WorkflowOrchestrator_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }
        
        [FunctionName("ExecuteWorkflow")]
        public static async Task<List<string>> ExecuteWorkflow([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
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
            var instanceId = await starter.StartNewAsync("ExecuteWorkflow", null, 1);
            //var workflowToStart = "Function2";

            // Function input comes from the request content.
            //string instanceId = await starter.StartNewAsync(workflowToStart, null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}