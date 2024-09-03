namespace generator.tests;

public class DiagnosticVerifier<TGenerator> : VerifierBase<TGenerator> where TGenerator : IIncrementalGenerator, new() {
    // The input code to run through the generator.
    public required string InputCode { get; init; }

    // List of error IDs to ignore. CS5001 is "Program does not contain a static 'Main' method suitable for an entry point".
    public IList<string> IgnoredErrors { get; init; } = ["CS5001"];

    // Runs the verifier. The action will receive the diagnostics emitted by the generator.
    public void Run(Action<IEnumerable<Diagnostic>> tester) {
        // Input validation omitted for brevity.

        // Compile the input code (the result is not used).
        _ = RunGenerator(InputCode, out var outputCompilation, out var diagnostics);

        // We get diagnostics if there are problems with the input source code. Fail on any
        // non-ignored error.
        var errors = outputCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error && !IgnoredErrors.Contains(d.Id)).ToList();
        Assert.That(errors, Is.Empty);

        // Test the emitted diagnostics
        tester(diagnostics);
    }
}