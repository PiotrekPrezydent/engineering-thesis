namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartCreatingExecutionPlan;

public interface IExecutionPlanGeneratorFactory
{
    public IExecutionPlanGeneratorService GetExecutionPlanGenerator(string providerName);
}