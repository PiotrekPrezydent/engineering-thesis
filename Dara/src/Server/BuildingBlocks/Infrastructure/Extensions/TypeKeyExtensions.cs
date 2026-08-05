using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;

namespace Dara.Server.BuildingBlocks.Infrastructure.Extensions;

public static class TypeKeyExtensions
{
    extension<T>(T) where T : class
    {
        public static ITypeKey<T> ToTypeKey()
        {
            return ITypeKey<T>.Instance;
        }
    }
}