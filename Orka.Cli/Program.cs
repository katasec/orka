using Orka.Cli.Config;
using Orka.Cli.Executor;
using Orka.Cli.Resources;

Console.WriteLine("Starting Orka...");

// Load and parse the workflow (from Bicep)
var workflow = WorkflowLoader.Load("orka.bicep");

// Execute the workflow
var executor = new WorkflowExecutor();
await executor.ExecuteWorkflowAsync(workflow);

Console.WriteLine("Finished.");