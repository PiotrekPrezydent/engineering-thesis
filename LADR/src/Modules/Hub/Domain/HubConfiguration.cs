using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Hub.Domain;

public class HubConfiguration : ValueObject
{
    public String Name { get; private set; }
    
    public String ConnectionCode { get; private set; }

    public String LLMModelApiKey {get; private set; }

    public Guid LLMModelId { get; private set; }
    
    public Guid LLMModelVersionId { get; private set; }
}