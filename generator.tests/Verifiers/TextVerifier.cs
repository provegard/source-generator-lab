namespace generator.tests;

public class TextVerifier<TGenerator> : VerifierBase<TGenerator> where TGenerator : IIncrementalGenerator, new() {
    // The input code to run through the generator.
    public required string InputCode { get; init; }

    // The sources we expect to be generated, as a list of tuples. The hint name is
    // typically the filename of a generated source file.
    public required (string hintName, string expectedCode)[] ExpectedCode { get; init; }

    // Whether to ignore line comments when comparing actual and expected source code.
    public bool IgnoreLineComments { get; init; } = true;

    // List of error IDs to ignore. CS5001 is "Program does not contain a static 'Main' method suitable for an entry point".
    public IList<string> IgnoredErrors { get; init; } = ["CS5001"];

    // Runs the verifier.
    public void Run() {
        // Input validation omitted for brevity.

        // Compile the input code.
        var runResult = RunGenerator(InputCode, out var outputCompilation, out var diagnostics);

        // We get diagnostics at this point if there are problems with the input source code. Fail on any
        // non-ignored error.
        var errors = outputCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error && !IgnoredErrors.Contains(d.Id)).ToList();
        Assert.That(errors, Is.Empty);

        // We'll see a diagnostic at this point if the generator throws an exception,
        // or if the generator emits a diagnostic.
        Assert.That(diagnostics, Is.Empty);

        // Get the result from the generator.
        var generatorResult = runResult.Results.FirstOrDefault();

        // Iterate over the expected source code files.
        foreach (var (hintName, code) in ExpectedCode) {
            // GeneratedSourceResult is a struct, so check that we get a valid result.
            var generated = generatorResult.GeneratedSources.FirstOrDefault(r => r.HintName == hintName);
            if (generated.HintName != hintName) {
                throw new InvalidOperationException($"No generated source found for: {hintName}");
            }

            // Convert the expected code into source text.
            var expectedSourceText = SourceText.From(code, Encoding.UTF8, SourceHashAlgorithm.Sha256);

            // Extract lines from the generated and expected source texts, for comparison.
            var generatedLines = getLines(generated.SourceText);
            var expectedLines = getLines(expectedSourceText);

            // Compare the lines using a collection assertion, to get as good failure
            // output as possible.
            Assert.That(generatedLines, Is.EqualTo(expectedLines), () => $"Line mismatch for {hintName}");
        }

        return;

        // This local function extracts lines from a source text, ignoring empty lines and
        // possibly ignoring line comments. Lines are trimmed so that comparison is not sensitive
        // to trailing whitespace.
        IList<string> getLines(SourceText text)
            => text.Lines
                .Select(l => l.ToString().Trim())
                .Where(l => l != "")
                .Where(l => !IgnoreLineComments || !l.TrimStart().StartsWith("//"))
                .ToList();
    }
}
