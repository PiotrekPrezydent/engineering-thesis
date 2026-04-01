using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Reasoning.Domain;

public class LLMModelVersion : Entity
{
    public string Name { get; private set; }
    public string Version { get; private set; }
}