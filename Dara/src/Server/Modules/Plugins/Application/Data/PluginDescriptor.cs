namespace Dara.Server.Modules.Plugins.Application.Data;

public record PluginDescriptor(PluginData Data, IReadOnlyList<PluginFunctionDescriptor> Functions)
{
    public static PluginDescriptorBuilder Builder => new();
}

public class PluginDescriptorBuilder
{
    private string _name;
    private string _description;
    private List<PluginFunctionDescriptor> _functions;
    public PluginDescriptorBuilder()
    {
        _functions = new();
        _name = "";
        _description = "";
    }

    public PluginDescriptor Build() => new PluginDescriptor(new PluginData(_name, _description), _functions);
    
    public PluginDescriptorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PluginDescriptorBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }
    

    public PluginDescriptorBuilder AddFunction(Action<PluginFunctionDescriptorBuilder> builder)
    {
        var configuration = PluginFunctionDescriptor.Builder;
        builder(configuration);
        
        _functions.Add(configuration.Build());
        return this;
    }
}