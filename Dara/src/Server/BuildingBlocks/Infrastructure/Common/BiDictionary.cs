namespace Dara.Server.BuildingBlocks.Infrastructure.Common;

public class BiDictionary<TFirst, TSecond> where TFirst : notnull where TSecond : notnull
{
    private readonly Dictionary<TFirst, TSecond> _forward = new();
    private readonly Dictionary<TSecond, TFirst> _reverse = new();
    
    public void Add(TFirst first, TSecond second)
    {
        if (_forward.ContainsKey(first))
            throw new ArgumentException($"{first} already exists");
        if (_reverse.ContainsKey(second))
            throw new ArgumentException($"{second} already exists");

        _forward.Add(first, second);
        _reverse.Add(second, first);
    }
    
    public TSecond GetByFirst(TFirst first) => _forward[first];
    
    public TFirst GetBySecond(TSecond second) => _reverse[second];
}