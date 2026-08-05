using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public record GenericTypeParameterData(GenericConstraints Constraints, EquatableArray<string> ConstraintTypesFullNames);