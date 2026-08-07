using System.Reflection;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;

namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;

public static class ObjectExtensions
{
    extension<T>(T) where T : class
    {
        public static ITypeKey<T> AsTypeKey =>ITypeKey<T>.Instance;

        public static Assembly ContainingAssembly => typeof(T).Assembly;
    }
}