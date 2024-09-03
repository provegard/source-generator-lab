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
}
