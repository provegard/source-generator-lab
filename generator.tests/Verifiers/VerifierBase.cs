using System.Collections.Immutable;

namespace generator.tests;

public abstract class VerifierBase<TGenerator> where TGenerator : IIncrementalGenerator, new() {
    private static readonly MetadataReference SystemRuntimeReference =
        MetadataReference.CreateFromFile(Assembly
            .Load("System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
    private static readonly MetadataReference NetStandard =
        MetadataReference.CreateFromFile(Assembly
            .Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location);
    private static readonly MetadataReference SystemPrivateCoreLib =
        MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location);
    private static readonly MetadataReference Generator =
        MetadataReference.CreateFromFile(typeof(TGenerator).GetTypeInfo().Assembly.Location);

    private static Compilation CreateCompilation(string source) =>
        CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source)],
            [
                SystemPrivateCoreLib,
                SystemRuntimeReference,
                NetStandard,
                Generator,
            ],
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

    protected GeneratorDriverRunResult RunGenerator(string inputCode, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics) {
        // Create the input compilation that the generator will act on.
        var inputCompilation = CreateCompilation(inputCode);

        // Create an instance of the generator.
        var generator = new TGenerator();

        // Create the driver that will control the generation.
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass. The driver is immutable, and all calls return a new driver instance.
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out outputCompilation,
            out diagnostics);

        return driver.GetRunResult();
    }
}