using System.Reflection;

namespace Dara.Shared.Common.Exceptions;

public class RequestParameterParseException(ParameterInfo parameter, object providedArg) : Exception
{
    
}