using colormodels;

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

    [Test]
    public void Should_generate_an_accessor_for_a_color_model_that_uses_String_with_leading_uppercase()
    {
        var type = Type.GetType("colormodels.StringUpperColorModelAccessor");
        Assert.That(type, Is.Not.Null);
    }

    [Test]
    public void Should_generate_a_property_for_a_color_model_component()
    {
        dynamic accessor = new SimpleColorModelAccessor(new Dictionary<string, double>());
        object value = accessor.Red;
        Assert.That(value, Is.Null);
    }

    [ColorModels]
    public class ColorModels {
        public static string[] Empty = [];
        public static String[] StringUpper = [];
        public static int[] Numeric = [];
        private static string[] Private = [];
        public string[] NonStatic = [];
        public static string[] Simple = ["red"];
    }
}
