using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderCollections;

public static class BuilderCollection
{
    public static FileDeclaration GetFileDeclaration()
    {
        var text = 
            $$""""
            {{BuilderCollectionNames.Usings}}
            
            namespace {{BuilderCollectionNames.Namespace}}
            {
                public class {{BuilderCollectionNames.ClassName}}<T>
                {
                    protected readonly ICollection<T> _collection;
                
                    public {{BuilderCollectionNames.ClassName}}(ICollection<T> collection)
                    {
                        _collection = collection;
                    }
                
                    public {{BuilderCollectionNames.ClassName}}<T> Add(T item)
                    {
                        _collection.Add(item);
                        return this;
                    }
                    
                    public {{BuilderCollectionNames.ClassName}}<T> AddRange(IEnumerable<T> items)
                    {
                        if (items == null) return this;
            
                        if (_collection is List<T> list)
                        {
                            list.AddRange(items);
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                _collection.Add(item);
                            }
                        }
            
                        return this;
                    }
                    
                    public {{BuilderCollectionNames.ClassName}}<T> AddRange(params T[] items)
                    {
                        return AddRange((IEnumerable<T>)items); 
                    }
                    
                }
            }
            """";
        return new FileDeclaration(BuilderCollectionNames.ClassName, text);
    }
}