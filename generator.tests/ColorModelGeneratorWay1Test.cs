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

    [ColorModels]
    public static class ColorModels {
        public static string[] Empty = [];
    }
}
