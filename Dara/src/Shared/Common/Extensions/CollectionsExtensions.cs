namespace Dara.Shared.Common.Extensions;

public static class CollectionsExtensions
{
    public static string ElementsToString<T>(this IEnumerable<T> enumerable)
    {
        return string.Join(Environment.NewLine, enumerable);
    }

    public static IEnumerable<KeyValuePair<Type, object>> ValuesWithType<T>(this IEnumerable<T> enumerable)
    {
        List<KeyValuePair<Type, object>> result = new();
        foreach (var element in enumerable)
        {
            result.Add(new(element.GetType(), element));
        }

        return result;
    }
}