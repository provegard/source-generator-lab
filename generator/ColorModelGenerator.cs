using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace generator;

[Generator]
public class ColorModelGenerator : IIncrementalGenerator
{
    private static readonly string[] TrademarkedColors = ["Vantablack"];
    private const string Namespace = "colormodels";

    private static readonly DiagnosticDescriptor TrademarkedColorDescriptor = new(
        id: "CM001",
        title: "Trademarked Color",
        messageFormat: "The color '{0}' in the '{1}' color model is trademarked",
        category: "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Define a syntax provider to filter nodes based on the ColorModels attribute
        // and extract the color model variables.
        // The predicate is not needed, so it just returns true.
        var variablesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                typeof(ColorModelsAttribute).FullName!,
                predicate: (_, _) => true,
                transform: (ctx, _) => GetVariables(ctx))
            .SelectMany((fields, _) => fields)
            .Collect();

        // Register the source output. The delegate receives the result from executing the provider.
        context.RegisterSourceOutput(variablesProvider, (spc, variables) => Execute(spc, variables));

        // Optional - generate the marker attribute instead of defining it in the generator assembly.
//         context.RegisterPostInitializationOutput(ctx => ctx.AddSource("attribute.g.cs", $"""
//              using System;
//              namespace {Namespace};
//              [AttributeUsage(AttributeTargets.Class)]
//              internal class {AttributeName} : Attribute;
//              """));
    }

    private static IEnumerable<VariableDeclaratorSyntax> GetVariables(GeneratorAttributeSyntaxContext context) {
        if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax) yield break;

        var fields = classDeclarationSyntax.Members
            .OfType<FieldDeclarationSyntax>()
            .Where(field =>
                field.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                field.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                field.Declaration.Type is ArrayTypeSyntax arrayType &&
                arrayType.ElementType.ToString() == "string"
            );
        foreach (var fieldDecl in fields) {
            foreach (var variable in fieldDecl.Declaration.Variables) {
                yield return variable;
            }
        }
    }

    private static void Execute(SourceProductionContext context, IEnumerable<VariableDeclaratorSyntax> variables)
    {
        var sourceBuilder = new StringBuilder($"""
                                               #nullable enable
                                               using System;
                                               using System.Collections.Generic;

                                               namespace {Namespace};

                                               """);

         foreach (var variable in variables)
         {
             var colorNames = GetStrings(variable);
             var categoryName = variable.Identifier.Text;
             sourceBuilder.AppendLine($$"""
                                        public class {{categoryName}}ColorModelAccessor(IDictionary<string, double> values) {
                                        """);

             foreach (var color in colorNames)
             {
                 if (TrademarkedColors.Contains(color))
                 {
                     // Trademarked color found!
                     var diagnostic = Diagnostic.Create(
                         TrademarkedColorDescriptor,
                         variable.GetLocation(),
                         color, categoryName);

                     context.ReportDiagnostic(diagnostic);
                 }

                 //sourceBuilder.AppendLine($"""
                 //                              public double? {ToPropertyName(color)} => values.TryGetValue("{color}", out var value) ? value : null;
                 //                          """);
             }

             sourceBuilder.AppendLine("}");
         }


         var generatedCode = sourceBuilder.ToString();
         context.AddSource("colormodels.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
    }

    private static string[] GetStrings(VariableDeclaratorSyntax variable)
     {
         var initializerValue = variable.Initializer?.Value;
         if (initializerValue is CollectionExpressionSyntax collection)
         {
             return collection.Elements.SelectMany(elem =>
                 elem is ExpressionElementSyntax { Expression: LiteralExpressionSyntax literal }
                     ? [literal.Token.ValueText]
                     : Array.Empty<string>()
             ).ToArray();
         }

         return [];
     }
}