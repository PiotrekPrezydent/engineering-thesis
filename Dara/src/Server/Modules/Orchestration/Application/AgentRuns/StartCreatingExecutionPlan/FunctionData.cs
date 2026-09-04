namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartCreatingExecutionPlan;

public class FunctionData
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<ParameterData> Parameters { get; set; }
    
    public string ReturnType { get; set; }
}