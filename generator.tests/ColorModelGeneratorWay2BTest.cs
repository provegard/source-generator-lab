using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace generator.tests;

[TestFixture]
public class ColorModelGeneratorWay2BTest
{
    [Test]
    public void Should_generate_an_accessor_class_for_a_color_model()
    {
        var verifier = new SyntaxVerifier<ColorModelGenerator>
        {
            InputCode = """
                        [generator.ColorModels]
                        public static class ColorModels {
                            public static string[] Empty = [];
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
}
