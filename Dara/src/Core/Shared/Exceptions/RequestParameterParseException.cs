using System.Reflection;

namespace Dara.Core.Shared.Exceptions;

public class RequestParameterParseException(ParameterInfo parameter, object providedArg) : Exception
{
    
}