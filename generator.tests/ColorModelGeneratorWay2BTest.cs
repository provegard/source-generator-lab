using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace generator.tests;

[TestFixture]
public class ColorModelGeneratorWay2BTest
{
    [TestCase("public static string")]
    [TestCase("public static System.String")]
    public void Should_generate_an_accessor_class_for_a_color_model(string modifiers)
    {
        var verifier = new SyntaxVerifier<ColorModelGenerator>
        {
            InputCode = $$"""
                          [generator.ColorModels]
                          public static class ColorModels {
                              {{modifiers}}[] Empty = [];
                          }
                          """,

            CodeTesters =
            [
                (
                    "colormodels.g.cs",
                    tree =>
                    {
                        var classNames = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                            .Select(cls => cls.Identifier.Text).ToList();
                        Assert.That(classNames, Does.Contain("EmptyColorModelAccessor"));
                    }
                )
            ]
        };

        verifier.Run();
    }

    [Test]
    public void Should_not_generate_an_accessor_for_a_non_public_static_string_color_model()
    {
        var verifier = new SyntaxVerifier<ColorModelGenerator>
        {
            InputCode = """
                        [generator.ColorModels]
                        public class ColorModels {
                            private static string[] Private = [];
                            public string[] NonStatic = [];
                            public static int[] Numeric = [];
                        }
                        """,

            CodeTesters =
            [
                (
                    "colormodels.g.cs",
                    tree =>
                    {
                        var classNames = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                            .Select(cls => cls.Identifier.Text).ToList();
                        Assert.That(classNames, Is.Empty);
                    }
                )
            ]
        };

        verifier.Run();
    }

    [TestCase("red")]
    [TestCase("RED")]
    [TestCase("Red")]
    public void Should_generate_a_property_for_a_color_model_component(string componentName)
    {
        var verifier = new SyntaxVerifier<ColorModelGenerator>
        {
            InputCode = $$"""
                          [generator.ColorModels]
                          public static class ColorModels {
                              public static string[] Simple = ["{{componentName}}"];
                          }
                          """,

            CodeTesters =
            [
                (
                    "colormodels.g.cs",
                    tree =>
                    {
                        var propertyNames = tree.GetRoot().DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .Where(cls => cls.Identifier.Text == "SimpleColorModelAccessor")
                            .SelectMany(cls => cls.Members)
                            .OfType<PropertyDeclarationSyntax>()
                            .Select(p => p.Identifier.Text)
                            .ToList();
                        Assert.That(propertyNames, Is.EqualTo(new[] { "Red" }));
                    }
                )
            ]
        };

        verifier.Run();
    }
}