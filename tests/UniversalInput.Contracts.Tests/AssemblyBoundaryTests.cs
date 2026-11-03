using NUnit.Framework;

namespace UniversalInput.Contracts.Tests;

[TestFixture]
public sealed class AssemblyBoundaryTests
{
    [TestCase("UniversalInput.Contracts.UniversalDiagnosticsUpdatedEventArgs")]
    [TestCase("UniversalInput.Contracts.EditorEnums.EditorDiagnosticSeverity")]
    [TestCase("UniversalInput.Contracts.UniversalEditorValueChangedEventArgs")]
    [TestCase("UniversalInput.Contracts.UniversalEditorValueChangeKind")]
    public void ProductContracts_AreAbsentFromSharedInputAssembly(string name)
    {
        Assert.That(typeof(UniversalPoint).Assembly.GetType(name), Is.Null);
    }

    [Test]
    public void ContractAssembly_ReferencesOnlyNetStandard()
    {
        var references = typeof(UniversalPoint).Assembly.GetReferencedAssemblies().Select(x => x.Name);
        Assert.That(references, Is.EquivalentTo(new[] { "netstandard" }));
    }
}
