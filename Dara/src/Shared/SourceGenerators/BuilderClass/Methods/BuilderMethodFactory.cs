namespace Dara.Shared.SourceGenerators.BuilderClass.Methods;

public class BuilderMethodFactory
{
    private readonly string _builderClassName;
    private readonly string _instanceFieldName;

    public BuilderMethodFactory(string builderClassName, string instanceFieldName)
    {
        _builderClassName = builderClassName;
        _instanceFieldName = instanceFieldName;
    }

    public string GetMethodCodeByType(IBuilderMethodData data)
    {
        return data switch
        {
            BaseMethodData baseMethodData => CreateBaseMethodCode(baseMethodData),
            CollectionMethodData collectionMethodData => CreateCollectionMethodCode(collectionMethodData),
            GenericMethodData genericMethodData => CreateGenericMethodCode(genericMethodData),
            _ => ""
        };
    }
    

    public string CreateBaseMethodCode(BaseMethodData data)
    {
        var text = 
            $$""""
            public {{_builderClassName}} {{data.MethodName}}({{data.ParameterType}} {{data.ParameterName}})
            {
                {{_instanceFieldName}}.{{data.PropertyName}} = {{data.ParameterName}};
                return this;
            }
            """";
        return text;
    }

    public string CreateGenericMethodCode(GenericMethodData data)
    {
        var baseData = data.BaseMethodData;
        var text =
            $$""""
            public {{_builderClassName}} {{baseData.MethodName}}{{data.ArgumentsDeclaration}}({{baseData.ParameterType}} {{baseData.ParameterName}}) {{data.WhereStatement}}
            {
                {{_instanceFieldName}}.{{baseData.PropertyName}} = {{baseData.ParameterName}};
                return this;
            }
            """";
        return text;
    }
    
    public string CreateCollectionMethodCode(CollectionMethodData data)
    {
        var baseData = data.BaseMethodData;
        var text =
            $$""""
            public {{_builderClassName}} {{baseData.MethodName}}(Action<{{baseData.ParameterType}}> {{baseData.ParameterName}})
            {
                var list = new List<{{data.CollectionParameterName}}>();
                var collectionBuilder = new {{baseData.ParameterType}}(list);
                
                {{baseData.ParameterName}}(collectionBuilder);
                
                {{_instanceFieldName}}.{{baseData.PropertyName}} = list;
                
                return this;
            }
            """";
        return text;
    }
}