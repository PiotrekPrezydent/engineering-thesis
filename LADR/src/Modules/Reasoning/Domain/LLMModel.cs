using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Reasoning.Domain;

public class LLMModel : Entity
{
    public string Name { get; private set; }
}