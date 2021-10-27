using System.Collections.Generic;

namespace TestDurableFunctions
{
    public class WorkflowManager
    {

        private Dictionary<int, Workflow> _workflows;

        public WorkflowManager()
        {
            _workflows = new Dictionary<int, Workflow>();
            Init();
        }


        private void Init()
        {
            var actionsForWorkflow1 = new List<WorkflowAction>()
            {
                new WorkflowAction("Function1_Hello"),
                new WorkflowAction("Function1_Hello_In_Spanish"),
                new WorkflowAction("Function1_Hello_In_Danish"),
            };

            var workflow1 = new Workflow("Workflow 1", actionsForWorkflow1);

            var actionsForWorkflow2 = new List<WorkflowAction>()
            {
                new WorkflowAction("Function1_Hello_In_Spanish"),
                new WorkflowAction("Function1_Hello"),
                new WorkflowAction("Function1_Hello_In_Danish"),
            };

            var workflow2 = new Workflow("Workflow 2", actionsForWorkflow2);

            var actionsForWorkflow3 = new List<WorkflowAction>()
            {
                new WorkflowAction("Function1_Hello_In_Spanish"),
                new WorkflowAction("Function1_Hello_In_Danish"),
                new WorkflowAction("Function1_Hello"),
            };

            var workflow3 = new Workflow("Workflow 3", actionsForWorkflow3);

            _workflows.Add(1, workflow1);
            _workflows.Add(2, workflow2);
            _workflows.Add(3, workflow3);
        }

        internal Workflow GetWorkflowByKey(int id)
        {
            return _workflows[id];
        }

    }
}