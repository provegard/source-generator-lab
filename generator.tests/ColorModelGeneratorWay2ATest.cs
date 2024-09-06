namespace generator.tests;

[TestFixture]
public class ColorModelGeneratorWay2ATest
{
    [Test]
    public void Should_generate_an_accessor_class_for_a_color_model()
    {
        var verifier = new TextVerifier<ColorModelGenerator>
        {
            InputCode = """
                        [generator.ColorModels]
                        public static class ColorModels {
                            public static string[] Empty = [];
                        }
                        """,

            ExpectedCode =
            [
                (
                    "colormodels.g.cs",
                    """
                    #nullable enable
                    using System;
                    using System.Collections.Generic;

                    namespace colormodels;

                    public class EmptyColorModelAccessor(IDictionary<string, double> values) {
                    }                                          
                    """)
            ]
        };

        verifier.Run();
    }

    [Test]
    public void Should_not_generate_an_accessor_for_a_non_public_static_string_color_model()
    {
        var verifier = new TextVerifier<ColorModelGenerator>
        {
            InputCode = """
                        [generator.ColorModels]
                        public class ColorModels {
                            private static string[] Private = [];
                            public string[] NonStatic = [];
                            public static int[] Numeric = [];
                        }
                        """,

            ExpectedCode =
            [
                (
                    "colormodels.g.cs",
                    """
                    #nullable enable
                    using System;
                    using System.Collections.Generic;

                    namespace colormodels;
                    """)
            ]
        };

        verifier.Run();
    }
}