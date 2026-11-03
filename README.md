# UniversalInput.Contracts

Platform-neutral .NET input and interaction contracts for reusable components.
PhialeTech and PhialeGis consume the same package; this library does not depend
on either product or on a UI framework. Target: `netstandard2.0`.

## Install

```xml
<PackageReference Include="UniversalInput.Contracts" Version="3.0.0-preview.3" />
```

This preview is prepared locally. Public availability on nuget.org is not implied.
Configure a feed containing the package before restoring a consumer.

## Use

```csharp
using UniversalInput.Contracts;

var input = new UniversalPointerRoutedEventArgs(
    new UniversalPointer(DeviceType.Mouse, new UniversalPoint { X = 12.5, Y = 30 })
    {
        Properties = new UniversalPointerPointProperties { IsLeftButtonPressed = true }
    })
{
    Metadata = new UniversalMetadata { ChangedButton = UniversalPointerButton.Left }
};
// Pass input to your component's input API from its platform adapter.
```

The package contains DTOs, enums, snapshot interfaces and an asynchronous command
gateway with cancellation. It does not install event handlers, recognize gestures,
render controls or implement component behavior. Native WPF/Avalonia/WinUI
adapters belong to consuming components and are not included here.

## Compatibility and scope

Version 3 moves product-specific diagnostics and cell-value contracts out of
UniversalInput. These retain their names and behavior in their owning assemblies:

| Types | New namespace / assembly |
| --- | --- |
| `UniversalDiagnosticsUpdatedEventArgs`, `EditorDiagnosticSeverity` | `PhialeGis.Library.Abstractions.Interactions.Editors` / `PhialeGis.Library.Abstractions` |
| `UniversalEditorValueChangedEventArgs`, `UniversalEditorValueChangeKind` | `PhialeGrid.Contracts` / `PhialeGrid.Contracts` (NuGet `PhialeTech.Grid.Contracts`) |

Consumers of these types must update their imports and rebuild with the matching
component packages. Existing binaries compiled against UniversalInput 2.x must
keep their old dependency set until rebuilt; this is a breaking preview change.
No forwarding dependency from UniversalInput back to products is introduced.
This change does not establish complete feature parity across platform adapters.

- `UniversalPoint` stores coordinates supplied by the adapter; it does not convert
  coordinate systems or DPI.
- Key and command identifiers are strings interpreted by the consuming component.
- Most event `Metadata` and pointer `Properties` may be null unless initialized
  by the adapter. Command metadata is always non-null.
- Command `Ctrl/Alt/Shift` are aliases of the matching `Metadata.Modifiers` bits.
  Replacing command metadata replaces the modifier state; assigning null resets
  it to a new empty metadata object. Other bits, including Windows/Meta, survive
  changes to an individual Boolean alias.
- Tap, double-tap and holding have overloads accepting an explicit `DeviceType`.
  Legacy overloads without a device retain the Mouse assumption. Adapters should
  supply the actual device, or Other when the platform event does not identify it.
- Selection `IsEmpty` is computed from `Start == End`; its setter was removed in
  preview.3. Consumers that assigned that property must use the endpoints instead.
- Commands may carry product-owned objects through `Parameter`; this is not a
  serialization or network protocol contract.
- Nullable reference annotations are disabled in the library. Constructors for
  text, query, replacement, URL, reason, key and command identifiers normalize
  null to an empty string; language and theme constructors use `plaintext` and
  `default`. Writable string properties can subsequently be assigned null.
  Pointer event constructors store the supplied pointer without a null guard;
  accessing `PointerDeviceType` on a null pointer throws `NullReferenceException`.
  Supply a non-null pointer. Other reference payloads are stored without deep
  copying or general validation. Gateway implementations own snapshot nullability,
  lifetime, revision policy and cancellation behavior.
- Reference payloads (including command metadata, `Parameter`, pointers and
  manipulation objects) remain shared with the caller. `Arguments` is a fresh,
  mutable dictionary per command, exposed through a getter. `UniversalPoint`
  and `UniversalVector` are value types and are copied by value. There is no
  ownership transfer, disposal, automatic cloning or immutable snapshot guarantee.
- Events and metadata are mutable and are not thread-safe. Producers initialize
  payloads before dispatch. Handlers treat published data as read-only, except
  intentional routing feedback such as Handled. Retaining data for asynchronous
  use requires copying the values and any referenced mutable data. Dispatch,
  synchronization and wheel units are owned by the component and its adapter.

## Build independently

From the repository root, use the .NET 10 SDK (selected by `global.json`)
and the .NET 8 runtime for the contract tests. No product checkout or third-party
runtime packages are required. `src/UniversalInput.Contracts/Version.props` is
the only explicit project import. The root README and license are also packed;
keep them with the source when copying the library.

```sh
dotnet restore UniversalInput.slnx
dotnet build UniversalInput.slnx -c Release --no-restore
dotnet test UniversalInput.slnx -c Release --no-build --no-restore
dotnet pack src/UniversalInput.Contracts/UniversalInput.Contracts.csproj -c Release --no-build --no-restore -o ./artifacts/packages
```

`Version.props` owns the independent preview version. Assembly identity remains
`UniversalInput.Contracts`, version `3.0.0.0`. Runtime dependencies: .NET Standard
only.

## License

UniversalInput.Contracts is distributed under the [MIT License](LICENSE.txt).
Copyright (c) 2026 Roland Rusiecki.

This library was extracted from PhialeTech/PhialeGis. This license covers this
library; it does not extend to other products from the original repository.
