using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderCollections;

public static class BuilderCollectionFileFactory
{
    public const string ClassName = "BuilderCollection";
    public const string TypeIgnoringClassName = "TypeIgnoring";
    public const string Namespace = "Dara.Shared.SourceGenerators.BuilderCollections";

    public static string GetFormatableName(bool typeIgnoring = false)
    {
        return Namespace + "."  + ClassName+"<{0}>";
    }
    
    public static FileDeclaration GetBuilderCollectionFile()
    {
        var text = 
            $$""""
            using System;
            using System.Collections.Generic;
            
            namespace {{Namespace}}
            {
                public class {{ClassName}}<T>
                {
                    protected readonly ICollection<T> _collection;
                    
                    public {{ClassName}}(ICollection<T> collection)
                    {
                        _collection = collection;
                    }
                    
                    public {{ClassName}}<T> Add(T item)
                    {
                        _collection.Add(item);
                        return this;
                    }
                }
                
                /*public class {{TypeIgnoringClassName}}{{ClassName}}<T>
                {
                    protected readonly ICollection<T> _collection;
                
                    public {{TypeIgnoringClassName}}{{ClassName}}(ICollection<T> collection)
                    {
                        _collection = collection;
                    }
                    
                    [Obsolete]
                    public {{TypeIgnoringClassName}}{{ClassName}}<T> Add(T item)
                    {
                        _collection.Add(item);
                        return this;
                    }
                    
                    public {{TypeIgnoringClassName}}{{ClassName}}<T> Add<U>(U item) where U : T
                    {
                        _collection.Add(item);
                        return this;
                    }
                }*/
                
            }
            
            """";
        
        return new FileDeclaration("BuilderCollections", text);
    }
}