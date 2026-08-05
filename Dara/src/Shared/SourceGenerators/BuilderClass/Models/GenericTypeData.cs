using System.Collections.Generic;
using System.Linq;
using Dara.Shared.SourceGenerators.Common;
using Dara.Shared.SourceGenerators.Extensions;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public record GenericTypeData(
    string Name,
    string Namespace,
    GenericConstraints SatisfiedGenericConstraints,
    EquatableArray<TypeData> Arguments,
    EquatableArray<GenericTypeParameterData> TypeParameters) : TypeData(Name, Namespace, SatisfiedGenericConstraints)
{
    public int Arity => Arguments.Count;

    public bool IsCollection => Types.CollectionTypes.Any(e => e == base.FullName + "`" + Arity);

    public override string FullName => base.FullName + "<" + string.Join(", ", Arguments.Select(e => e.FullName)) + ">";

    public string FullNameParametless => base.FullName;

    public string TypeArgumentsString => "<" + string.Join(", ", Arguments.Select(e => e.FullName)) + ">";

    public string TypeArgumentsFormatableString =>
        "<" + string.Join(", ", Enumerable.Range(0, Arguments.Count).Select(i => "{" + i + "}")) + ">";


    public string FullNameWithArgumentName(string newArgumentName, int replacedArgumentIndex)
    {
        var returned = FullNameParametless;
        returned += "<";
        for (int i = 0; i < Arguments.Count; i++)
        {
            if (i == replacedArgumentIndex)
                returned += newArgumentName;
            else
                returned += Arguments[i].FullName;

            if (i < Arguments.Count - 1)
                returned += ", ";
        }

        returned += ">";
        return returned;
    }

    public string GetWhereStatemtForArgument(string typeParameterName, int argumentIndex)
    {
        var statemet = $"where {typeParameterName} : ";
        var parts = new List<string>();

        var arg = Arguments[argumentIndex];
        var par = TypeParameters[argumentIndex];
        var missingConstraints = par.Constraints & ~ arg.SatisfiedGenericConstraints;

        var constraintsArray = (missingConstraints & ~ GenericConstraints.New).FlagsToArray();

        foreach (var constrain in constraintsArray)
        {
            if (constrain == GenericConstraints.None)
                continue;
            parts.Add(constrain.ConstraintAsString());
        }

        parts.Add(arg.FullName);

        foreach (var constraintTypes in par.ConstraintTypesFullNames)
        {
            if (constraintTypes.Contains(arg.FullName))
                continue;

            parts.Add(constraintTypes);
        }

        if (missingConstraints.HasFlag(GenericConstraints.New))
            parts.Add(GenericConstraints.New.ConstraintAsString());

        statemet += string.Join(", ", parts);

        return statemet;
    }
}