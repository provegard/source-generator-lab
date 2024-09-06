namespace generator.tests;

[TestFixture]
public class ColorModelGeneratorWay1Test
{
    [Test]
    public void Should_generate_an_accessor_for_a_color_model()
    {
        var type = Type.GetType("colormodels.EmptyColorModelAccessor");
        Assert.That(type, Is.Not.Null);
    }

    [Test]
    public void Should_not_generate_an_accessor_for_a_private_color_model()
    {
        var type = Type.GetType("colormodels.PrivateColorModelAccessor");
        Assert.That(type, Is.Null);
    }

    [Test]
    public void Should_not_generate_an_accessor_for_a_non_static_color_model()
    {
        var type = Type.GetType("colormodels.NonStaticColorModelAccessor");
        Assert.That(type, Is.Null);
    }

    [Test]
    public void Should_not_generate_an_accessor_for_a_non_string_based_color_model()
    {
        var type = Type.GetType("colormodels.NumericColorModelAccessor");
        Assert.That(type, Is.Null);
    }

    [ColorModels]
    public class ColorModels {
        public static string[] Empty = [];
        public static int[] Numeric = [];
        private static string[] Private = [];
        public string[] NonStatic = [];
    }
}
