
using System;
using System.Text;
using Dara.Shared.SourceGenerators.Attributes;
using Dara.Shared.SourceGenerators.BuilderClass;
using Dara.Shared.SourceGenerators.BuilderCollections;
using Dara.Shared.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Dara.Shared.SourceGenerators;

[Generator]
public class BuilderSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        FileDeclaration[] attributesFiles =
        [
            AttributeClassFileFactory.CreateAttributeFileDeclaration(AttributeClassSpecification.GenerateBuilderAttributeSpecification),
            AttributeClassFileFactory.CreateAttributeFileDeclaration(AttributeClassSpecification.BuilderMethodNameAttributeSpecification),
            AttributeClassFileFactory.CreateAttributeFileDeclaration(AttributeClassSpecification.ObsoleteMethodOnRepeatedTypeAttributeClassSpecification)
        ];

        foreach (var file in attributesFiles)
        {
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource(file.FileNameWithSuffix, SourceText.From(file.Text, Encoding.UTF8)));
        }

        FileDeclaration[] builderCollections =
        [
            BuilderCollection.GetFileDeclaration(),
            TypeIgnoringBuilderCollection.GetFileDeclaration()
        ];
        
        foreach (var file in builderCollections)
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource(file.FileNameWithSuffix, SourceText.From(file.Text, Encoding.UTF8)));
        
        
        var classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeNames.AttributesNamespace + "." + AttributeNames.GenerateBuilderAttributeName,
                predicate: BuilderClassParser.IsClassTarget,
                transform: BuilderClassParser.Parse
            )
            .Where(static data => data is not null);
        
        context.RegisterSourceOutput(classDeclarations, static (spc, source) =>
        {
            try
            {
                var file = BuilderClassFileFactory.GenerateBuilderClassFileDeclaration(source);
                spc.AddSource(file.FileNameWithSuffix, SourceText.From(file.Text, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                spc.AddSource(source.BuilderClassName + "_ERROR.g.cs",
                    $"/* GENERATOR FAILED: {ex.Message} \n {ex.StackTrace} */");
            }
        });
    }
}


