using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dara.Shared.SourceGenerators.Common;

public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
{
    private readonly T[]? _collection;

    public EquatableArray(T[] collection) => _collection = collection;
    
    public bool Equals(EquatableArray<T> other)
    {
        if (ReferenceEquals(_collection, other._collection))
            return true;
        
        if (_collection is null || other._collection is null)
            return false;
         
        return _collection.SequenceEqual(other._collection);
    }

    public override bool Equals(object? obj) => 
        obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        if (_collection == null) return 0;
        int hash = 17;
        foreach (var item in _collection) hash = hash * 23 + (item?.GetHashCode() ?? 0);
        return hash;
    }
    
    public override string ToString() => string.Join(", ", _collection?.Select(e=>e?.ToString()) ?? Enumerable.Empty<string>());
    
    public int Count => _collection?.Length ?? 0;
    
    public T this[int index] => _collection![index];
    
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)(_collection ?? Array.Empty<T>())).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator EquatableArray<T>(T[] array) => new(array);
}