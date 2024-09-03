namespace generator.tests;

[TestFixture]
public class ColorModelGeneratorWay2CTest
{
    [Test]
    public void Should_detect_trademarked_color()
    {
        var verifier = new DiagnosticVerifier<ColorModelGenerator>
        {
            InputCode = """
                        [generator.ColorModels]
                        public static class ColorModels {
                            public static string[] Misc = ["red", "Vantablack"];
                        }
                        """
        };

        verifier.Run(diags =>
        {
            var messages = diags.Select(diag => diag.GetMessage()).ToList();
            Assert.That(messages, Does.Contain("The color 'Vantablack' in the 'Misc' color model is trademarked"));
        });
    }
}
