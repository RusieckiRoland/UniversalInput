using System.Reflection;
using NUnit.Framework;
using UniversalInput.Contracts;

namespace UniversalInput.Contracts.Tests;

[TestFixture]
public sealed class PublicInputContractTests
{
    public static IEnumerable<TestCaseData> Constructors()
    {
        yield return Api<UniversalPointer>(typeof(DeviceType), typeof(UniversalPoint));
        yield return Api<UniversalPointerRoutedEventArgs>(typeof(UniversalPointer));
        yield return Api<UniversalPointerCanceledEventArgs>(typeof(UniversalPointer), typeof(UniversalPointerCancelReason));
        yield return Api<UniversalPointerWheelChangedEventArgs>(typeof(int), typeof(UniversalPoint));
        yield return Api<UniversalManipulationStartingRoutedEventArgs>(typeof(DeviceType), typeof(UniversalPivot));
        yield return Api<UniversalCommandEventArgs>(typeof(string), typeof(bool), typeof(bool), typeof(bool));
        yield return Api<UniversalManipulationDelta>(typeof(UniversalPoint), typeof(float), typeof(float));
        yield return Api<UniversalManipulationStartedRoutedEventArgs>(typeof(UniversalPoint), typeof(DeviceType), typeof(UniversalManipulationDelta));
        yield return Api<UniversalManipulationDeltaRoutedEventArgs>(typeof(UniversalPoint), typeof(DeviceType), typeof(UniversalManipulationDelta), typeof(UniversalManipulationDelta));
        yield return Api<UniversalManipulationCompletedRoutedEventArgs>(typeof(bool), typeof(UniversalManipulationDelta), typeof(bool), typeof(DeviceType), typeof(UniversalPoint), typeof(UniversalManipulationVelocities));
        yield return Api<UniversalTextChangedEventArgs>(typeof(string));
        yield return Api<UniversalSelectionChangedEventArgs>(typeof(int), typeof(int), typeof(int), typeof(int));
        yield return Api<UniversalCaretMovedEventArgs>(typeof(int), typeof(int), typeof(int));
        yield return Api<UniversalDirtyChangedEventArgs>(typeof(bool));
        yield return Api<UniversalSaveRequestedEventArgs>(typeof(string));
        yield return Api<UniversalLanguageChangedEventArgs>(typeof(string));
        yield return Api<UniversalThemeChangedEventArgs>(typeof(string));
        yield return Api<UniversalFindRequestedEventArgs>(typeof(string), typeof(bool), typeof(bool), typeof(bool));
        yield return Api<UniversalReplaceRequestedEventArgs>(typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool));
        yield return Api<UniversalLinkClickedEventArgs>(typeof(string));
        yield return Api<UniversalHoverRequestedEventArgs>(typeof(int), typeof(int), typeof(int));
    }
    private static TestCaseData Api<T>(params Type[] arguments) => new TestCaseData(typeof(T), arguments).SetName("PublicConstructor_" + typeof(T).Name);

    [TestCaseSource(nameof(Constructors))]
    public void RequiredConstructor_RemainsPublicInSharedNamespace(Type type, Type[] parameters)
    {
        Assert.That(type.FullName, Is.EqualTo("UniversalInput.Contracts." + type.Name));
        Assert.That(type.IsPublic, Is.True);
        Assert.That(type.GetConstructor(parameters), Is.Not.Null);
    }

    [Test]
    public void UniversalPoint_ExposesDoubleCoordinatesWithoutAxisConversion()
    {
        Assert.That(typeof(UniversalPoint).GetField("X")!.FieldType, Is.EqualTo(typeof(double)));
        Assert.That(typeof(UniversalPoint).GetField("Y")!.FieldType, Is.EqualTo(typeof(double)));
        var point = new UniversalPoint { X = -12.75, Y = 31.125 };
        var copy = point; copy.X = 100;
        Assert.That(point.X, Is.EqualTo(-12.75)); Assert.That(point.Y, Is.EqualTo(31.125));
    }

    [TestCase(DeviceType.Mouse)] [TestCase(DeviceType.Pen)] [TestCase(DeviceType.Touch)]
    public void Pointer_PreservesDeviceIdFractionalPositionAndButtonState(DeviceType device)
    {
        var properties = new UniversalPointerPointProperties { IsLeftButtonPressed = true, IsRightButtonPressed = false, IsMiddleButtonPressed = true, Pressure = .625 };
        var pointer = new UniversalPointer(device, new UniversalPoint { X = -2.5, Y = 19.75 }) { PointerId = 73, Properties = properties };
        var args = new UniversalPointerRoutedEventArgs(pointer);
        Assert.That(args.Pointer, Is.SameAs(pointer)); Assert.That(args.PointerDeviceType, Is.EqualTo(device));
        Assert.That(pointer.PointerId, Is.EqualTo(73)); Assert.That(pointer.Position.X, Is.EqualTo(-2.5)); Assert.That(pointer.Position.Y, Is.EqualTo(19.75));
        Assert.That(pointer.Properties.IsLeftButtonPressed, Is.True); Assert.That(pointer.Properties.IsRightButtonPressed, Is.False);
        Assert.That(pointer.Properties.IsMiddleButtonPressed, Is.True); Assert.That(pointer.Properties.Pressure, Is.EqualTo(.625));
        Assert.That(args.Handled, Is.False); args.Handled = true; Assert.That(args.Handled, Is.True);
    }

    [Test]
    public void Command_PreservesIdModifiersParameterAndIndependentArguments()
    {
        var payload = new object(); var command = new UniversalCommandEventArgs("enter", true, false, true) { Parameter = payload };
        command.Arguments["scope"] = "selection";
        Assert.That(command.CommandId, Is.EqualTo("enter")); Assert.That(command.Ctrl, Is.True); Assert.That(command.Alt, Is.False); Assert.That(command.Shift, Is.True);
        Assert.That(command.Parameter, Is.SameAs(payload));
        Assert.That(command.Arguments["scope"], Is.EqualTo("selection"));
        Assert.That(new UniversalCommandEventArgs("save", false, true, false).Arguments, Is.Empty);
        Assert.That(typeof(UniversalCommandEventArgs).GetProperty("Arguments")!.PropertyType, Is.EqualTo(typeof(IDictionary<string, string>)));
    }

    [Test]
    public void RequiredProperties_RemainReadableWithThePublishedTypes()
    {
        var properties = new (Type Owner, string Name, Type Value)[] {
            (typeof(UniversalPointer), "Position", typeof(UniversalPoint)), (typeof(UniversalPointer), "PointerId", typeof(uint)),
            (typeof(UniversalPointer), "Properties", typeof(UniversalPointerPointProperties)), (typeof(UniversalPointer), "PointerDeviceType", typeof(DeviceType)),
            (typeof(UniversalPointerRoutedEventArgs), "Pointer", typeof(UniversalPointer)), (typeof(UniversalPointerRoutedEventArgs), "Handled", typeof(bool)),
            (typeof(UniversalManipulationDelta), "Translation", typeof(UniversalPoint)), (typeof(UniversalManipulationDelta), "Rotation", typeof(float)), (typeof(UniversalManipulationDelta), "Scale", typeof(float)),
            (typeof(UniversalCommandEventArgs), "CommandId", typeof(string)), (typeof(UniversalCommandEventArgs), "Parameter", typeof(object)),
            (typeof(UniversalTextChangedEventArgs), "Text", typeof(string)), (typeof(UniversalSelectionChangedEventArgs), "Start", typeof(int)), (typeof(UniversalSelectionChangedEventArgs), "End", typeof(int)),
            (typeof(UniversalCaretMovedEventArgs), "Offset", typeof(int)), (typeof(UniversalDirtyChangedEventArgs), "IsDirty", typeof(bool)),
            (typeof(UniversalSaveRequestedEventArgs), "Reason", typeof(string)), (typeof(UniversalLanguageChangedEventArgs), "LanguageId", typeof(string)),
            (typeof(UniversalThemeChangedEventArgs), "ThemeId", typeof(string)), (typeof(UniversalFindRequestedEventArgs), "Query", typeof(string)),
            (typeof(UniversalReplaceRequestedEventArgs), "Replacement", typeof(string)), (typeof(UniversalLinkClickedEventArgs), "Url", typeof(string)),
            (typeof(UniversalHoverRequestedEventArgs), "Offset", typeof(int)) };
        foreach (var (owner, name, value) in properties)
        {
            var property = owner.GetProperty(name);
            Assert.That(property?.PropertyType, Is.EqualTo(value), owner.Name + "." + name);
            Assert.That(property!.GetMethod!.IsPublic, Is.True);
        }
    }

    [Test]
    public void Command_ModifierCombinationsPreserveIndependentFlags([Range(0, 7)] int mask)
    {
        var command = new UniversalCommandEventArgs("action", (mask & 1) != 0, (mask & 2) != 0, (mask & 4) != 0);
        Assert.That(command.Ctrl, Is.EqualTo((mask & 1) != 0)); Assert.That(command.Alt, Is.EqualTo((mask & 2) != 0)); Assert.That(command.Shift, Is.EqualTo((mask & 4) != 0));
    }

    [TestCase(120)] [TestCase(-120)]
    public void WheelAndCancellation_PreserveDirectionPositionAndReason(int delta)
    {
        var wheel = new UniversalPointerWheelChangedEventArgs(delta, new UniversalPoint { X = 10.5, Y = -3.25 });
        Assert.That(wheel.Delta, Is.EqualTo(delta)); Assert.That(wheel.Position.Y, Is.EqualTo(-3.25)); Assert.That(wheel.PointerDeviceType, Is.EqualTo(DeviceType.Wheel));
        var pointer = new UniversalPointer(DeviceType.Pen, wheel.Position) { PointerId = 31 };
        var cancel = new UniversalPointerCanceledEventArgs(pointer, UniversalPointerCancelReason.FocusLost);
        Assert.That(cancel.Pointer, Is.SameAs(pointer)); Assert.That(cancel.Reason, Is.EqualTo(UniversalPointerCancelReason.FocusLost)); Assert.That(cancel.PointerDeviceType, Is.EqualTo(DeviceType.Pen));
    }

    [Test]
    public void Manipulation_DeltaCumulativeAndScreenPositionRemainDistinct()
    {
        var delta = new UniversalManipulationDelta(new UniversalPoint { X = -1.25, Y = 2.5 }, 15f, 1.5f);
        var cumulative = new UniversalManipulationDelta(new UniversalPoint { X = 20, Y = -40 }, 45f, 3f);
        var args = new UniversalManipulationDeltaRoutedEventArgs(new UniversalPoint { X = 10, Y = 30 }, DeviceType.Touch, delta, cumulative);
        Assert.That(args.Delta, Is.SameAs(delta)); Assert.That(args.Cumulative, Is.SameAs(cumulative));
        Assert.That(args.Position.X, Is.EqualTo(10)); Assert.That(args.Position.Y, Is.EqualTo(30));
        Assert.That(args.Delta.Translation.X, Is.EqualTo(-1.25)); Assert.That(args.Delta.Rotation, Is.EqualTo(15)); Assert.That(args.Delta.Scale, Is.EqualTo(1.5));
        Assert.That(args.Cumulative.Translation.Y, Is.EqualTo(-40)); Assert.That(args.PointerDeviceType, Is.EqualTo(DeviceType.Touch));
    }

}
