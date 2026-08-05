using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dara.Shared.SourceGenerators.Attributes;
using Dara.Shared.SourceGenerators.BuilderClass.Models;
using Dara.Shared.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dara.Shared.SourceGenerators.BuilderClass;

public static class BuilderClassParser
{
    public static bool IsClassTarget(SyntaxNode node, CancellationToken ct) => node is ClassDeclarationSyntax;
    
    public static BuilderClassSpecification? Parse(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;
        
        string className = classSymbol.Name;
        string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var propertiesBuilder = new List<PropertyData>();
        
        foreach (var member in classSymbol.GetMembers())
        {
            ct.ThrowIfCancellationRequested();
            
            if (member is not IPropertySymbol propertySymbol)
                continue;
            
            var data = ParsePropertySymbol(propertySymbol);
            
            propertiesBuilder.Add(data);
        }

        return new BuilderClassSpecification(namespaceName, className, propertiesBuilder.ToArray());
    }
    

    static PropertyData ParsePropertySymbol(IPropertySymbol propertySymbol)
    {
        var propName = propertySymbol.Name;
        var propTypeData = ParseTypeData(propertySymbol.Type);
        
        var propAttributes = propertySymbol.GetAttributes();
        var collectedAttributes = new List<PropertyAttributeData>();
        
        foreach (var attr in propAttributes)
        {
            if (attr.AttributeClass is null)
                continue;

            if (attr.AttributeClass.Name == AttributeNames.BuilderMethodNameAttributeName)
            {
                if(attr.ConstructorArguments.Length != 1)
                    continue;
                
                if(attr.ConstructorArguments[0].Value is not string name)
                    continue;
                
                collectedAttributes.Add(new CustomMethodNameAttributeData(name));
            }

            if (attr.AttributeClass.Name == AttributeNames.ObsoleteMethodOnRepeatedTypeAttributeName)
            {
                if(attr.ConstructorArguments.Length != 1)
                    continue;
                
                if(attr.ConstructorArguments[0].Value is not ITypeSymbol argTypeSymbol)
                    continue;
                
                var argTypeData = ParseTypeData(argTypeSymbol);
                
                collectedAttributes.Add(new ObsoleteMethodOnRepeatedTypeAttributeData(argTypeData));
            }
        }
        
        return new PropertyData(propName,propTypeData,collectedAttributes.ToArray());
    }

    static TypeData ParseTypeData(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            return new UnexpectedTypeData();

        
        string typeName = typeSymbol.Name;
        string typeNamespace = typeSymbol.ContainingNamespace.ToDisplayString();
        GenericConstraints mettedGenericConstraints = GetMettedGenericConstraintsForSymbol(namedTypeSymbol);
        
        if(!namedTypeSymbol.IsGenericType)
            return new SimpleTypeData(typeName, typeNamespace, mettedGenericConstraints);
        
        var args = namedTypeSymbol.TypeArguments;
        var pars = namedTypeSymbol.TypeParameters;
        
        if(args.Length != pars.Length)
            return new UnexpectedTypeData();
        
        var argsData = new List<TypeData>();
        var parsData = new List<GenericTypeParameterData>();
        
        for (int i = 0; i < args.Length; i++)
        {
            argsData.Add(ParseTypeData(args[i]));
            parsData.Add(ParseTypeParameterData(pars[i]));
        }
        
        return new GenericTypeData(typeName, typeNamespace, mettedGenericConstraints,argsData.ToArray(),parsData.ToArray());
    }

    static GenericConstraints GetMettedGenericConstraintsForSymbol(ITypeSymbol symbol)
    {
        var constraints = GenericConstraints.None;
        
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            if(namedTypeSymbol.Constructors.Any(e=>e.DeclaredAccessibility == Accessibility.Public && e.Parameters.IsEmpty))
                constraints |= GenericConstraints.New;
        }
        
        if (symbol.IsReferenceType && symbol.TypeKind != TypeKind.Interface)
            constraints |= GenericConstraints.Class;
        
        if (symbol.NullableAnnotation == NullableAnnotation.NotAnnotated)
            constraints |= GenericConstraints.NotNull;
        
        if (symbol.IsUnmanagedType)
            constraints |= GenericConstraints.Unmanaged;
        
        if(symbol.IsValueType && symbol.TypeKind == TypeKind.Struct)
            constraints |= GenericConstraints.Struct;
        
        return constraints;
    }

    static GenericTypeParameterData ParseTypeParameterData(ITypeParameterSymbol symbol)
    {
        var constraints = GenericConstraints.None;
        
        if (symbol.HasConstructorConstraint)
            constraints |= GenericConstraints.New;
        
        if(symbol.HasNotNullConstraint)
            constraints |= GenericConstraints.NotNull;
        
        if (symbol.HasReferenceTypeConstraint)
            constraints |= GenericConstraints.Class;
        
        if(symbol.HasUnmanagedTypeConstraint)
            constraints |= GenericConstraints.Unmanaged;
        
        if(symbol.HasValueTypeConstraint)
            constraints |= GenericConstraints.Struct;
        
        var constraintTypes = new List<string>();
        foreach (var type in symbol.ConstraintTypes)
        {
            constraintTypes.Add(type.ToDisplayString());
        }
        
        return new GenericTypeParameterData(constraints, constraintTypes.ToArray());
    }
}