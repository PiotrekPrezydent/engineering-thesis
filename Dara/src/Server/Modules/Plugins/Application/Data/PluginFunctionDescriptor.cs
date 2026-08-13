namespace Dara.Server.Modules.Plugins.Application.Data;

public record PluginFunctionDescriptor(PluginFunctionData Data, IReadOnlyList<PluginFunctionParameterData> Parameters)
{
    public static PluginFunctionDescriptorBuilder Builder => new PluginFunctionDescriptorBuilder();
}

public class PluginFunctionDescriptorBuilder
{
    private string _name;
    private string _description;
    private string _returnTypeName;
    List<PluginFunctionParameterData> _parameters;
 
    public PluginFunctionDescriptorBuilder()
    {
        _parameters = new();
        _name = "";
        _description = "";
        _returnTypeName = "";
    }
    public PluginFunctionDescriptorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PluginFunctionDescriptorBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public PluginFunctionDescriptorBuilder WithReturnTypeName(string returnTypeName)
    {
        _returnTypeName = returnTypeName;
        return this;
    }
    
    
    public PluginFunctionDescriptorBuilder AddParameter(Action<PluginFunctionParameterDataBuilder> parameter)
    {
        var builder = new PluginFunctionParameterDataBuilder();
        parameter(builder);
        _parameters.Add(builder.Build());
        return this;
    }
    
    public PluginFunctionDescriptor Build()
    {
        return new PluginFunctionDescriptor(new PluginFunctionData(_name,_description,_returnTypeName), _parameters);
    }
    
}

public class PluginFunctionParameterDataBuilder
{
    private string _name;
    private string _description;
    private string _typeName;

    public PluginFunctionParameterDataBuilder()
    {
        _name = "";
        _description = "";
        _typeName = "";
    }

    public PluginFunctionParameterDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PluginFunctionParameterDataBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public PluginFunctionParameterDataBuilder WithTypeName(string typeName)
    {
        _typeName = typeName;
        return this;
    }
    
    public PluginFunctionParameterData Build() => new(_name, _description, _typeName);
}