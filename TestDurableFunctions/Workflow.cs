using System.Collections.Generic;

namespace TestDurableFunctions
{
    public class Workflow
    {
        public Workflow()
        {
            Actions = new List<WorkflowAction>();
        }

        public Workflow(string name, List<WorkflowAction> actions)
        {
            Name = name;
            Actions = actions;
        }

        public string Name { get; set; }

        public List<WorkflowAction> Actions { get; set; }
    }

    public class WorkflowAction
    {

        public WorkflowAction(string workflowActionName)
        {
            WorkflowActionName = workflowActionName;
        }

        public string WorkflowActionName { get; set; }
    }
}