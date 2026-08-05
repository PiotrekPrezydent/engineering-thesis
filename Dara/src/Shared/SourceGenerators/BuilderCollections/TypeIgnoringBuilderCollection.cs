using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderCollections;

public static class TypeIgnoringBuilderCollection
{
    public static FileDeclaration GetFileDeclaration()
    {
        var text = 
            $$""""
              {{BuilderCollectionNames.Usings}}

              namespace {{BuilderCollectionNames.Namespace}}
              {
                  public class {{BuilderCollectionNames.TypeIgnoringClassName}}<T>
                  {
                      protected readonly ICollection<T> _collection;
                  
                      public {{BuilderCollectionNames.TypeIgnoringClassName}}(ICollection<T> collection)
                      {
                          _collection = collection;
                      }
                      
                      [Obsolete("",true)]
                      public {{BuilderCollectionNames.TypeIgnoringClassName}}<T> Add(T item)
                      {
                          _collection.Add(item);
                          return this;
                      }
                      
                      public {{BuilderCollectionNames.TypeIgnoringClassName}}<T> Add<U>(U item) where U : T
                      {
                        _collection.Add(item);
                        return this;
                      }
                      
                  }
              }
              """";
        return new FileDeclaration(BuilderCollectionNames.TypeIgnoringClassName, text);
    }
}