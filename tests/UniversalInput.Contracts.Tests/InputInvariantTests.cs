using NUnit.Framework;

namespace UniversalInput.Contracts.Tests;

[TestFixture]
public sealed class InputInvariantTests
{
    [TestCase(DeviceType.Mouse)]
    [TestCase(DeviceType.Pen)]
    [TestCase(DeviceType.Touch)]
    [TestCase(DeviceType.MultiTouch)]
    [TestCase(DeviceType.Wheel)]
    [TestCase(DeviceType.Other)]
    public void GestureOverloads_PreserveTheExplicitDeviceAndPosition(DeviceType device)
    {
        var point = new UniversalPoint { X = -12.5, Y = 7.25 };
        var tap = new UniversalTappedRoutedEventArgs(point, device);
        var doubleTap = new UniversalDoubleTappedRoutedEventArgs(point, device);
        var hold = new UniversalHoldingRoutedEventArgs(point, UniversalHoldingState.Completed, device);
        Assert.Multiple(() => {
            Assert.That(new[] { tap.PointerDeviceType, doubleTap.PointerDeviceType, hold.PointerDeviceType }, Is.All.EqualTo(device));
            Assert.That(new[] { tap.Position.X, doubleTap.Position.X, hold.Position.X }, Is.All.EqualTo(point.X));
            Assert.That(new[] { tap.Position.Y, doubleTap.Position.Y, hold.Position.Y }, Is.All.EqualTo(point.Y));
            Assert.That(hold.HoldingState, Is.EqualTo(UniversalHoldingState.Completed));
            Assert.That(new[] { tap.Handled, doubleTap.Handled, hold.Handled }, Is.All.False);
        });
    }

    [Test]
    public void LegacyGestureConstructors_KeepTheirDocumentedMouseDefault()
    {
        var point = new UniversalPoint();
        Assert.That(new IUniversalBase[] { new UniversalTappedRoutedEventArgs(point),
            new UniversalDoubleTappedRoutedEventArgs(point),
            new UniversalHoldingRoutedEventArgs(point, UniversalHoldingState.Started) }
            .Select(args => args.PointerDeviceType), Is.All.EqualTo(DeviceType.Mouse));
    }

    [Test]
    public void Selection_EmptinessFollowsBothEndpointMutations()
    {
        var args = new UniversalSelectionChangedEventArgs(0, 0, 1, 2);
        args.End = 10;
        Assert.That(args.IsEmpty, Is.False);
        args.Start = 10;
        Assert.That(args.IsEmpty, Is.True);
        args.Start = 3;
        Assert.That(args.IsEmpty, Is.False);
    }

    [Test]
    public void Command_ConstructorFlagsAreVisibleInMetadata()
    {
        var command = new UniversalCommandEventArgs("save", true, false, true);
        Assert.That(command.Metadata?.Modifiers, Is.EqualTo(UniversalModifierKeys.Control | UniversalModifierKeys.Shift));
    }

    [Test]
    public void Command_MetadataMutationUpdatesAllBooleanAliases()
    {
        var command = new UniversalCommandEventArgs("save", false, true, false)
        { Metadata = new UniversalMetadata { Modifiers = UniversalModifierKeys.Control | UniversalModifierKeys.Shift } };
        Assert.Multiple(() => {
            Assert.That(command.Ctrl, Is.True); Assert.That(command.Alt, Is.False); Assert.That(command.Shift, Is.True);
        });
        command.Metadata.Modifiers = UniversalModifierKeys.Alt;
        Assert.Multiple(() => {
            Assert.That(command.Ctrl, Is.False); Assert.That(command.Alt, Is.True); Assert.That(command.Shift, Is.False);
        });
    }

    [Test]
    public void Command_BooleanAliasesUpdateMetadataAndPreserveOtherBits()
    {
        var other = UniversalModifierKeys.Windows | (UniversalModifierKeys)128;
        var command = new UniversalCommandEventArgs("save", false, false, false)
        { Metadata = new UniversalMetadata { Modifiers = other } };
        command.Ctrl = true; command.Alt = true; command.Shift = true;
        Assert.That(command.Metadata.Modifiers, Is.EqualTo(other | UniversalModifierKeys.Control | UniversalModifierKeys.Alt | UniversalModifierKeys.Shift));
        command.Ctrl = false; command.Alt = false; command.Shift = false;
        Assert.That(command.Metadata.Modifiers, Is.EqualTo(other));
    }

    [Test]
    public void Command_ReplacingMetadataReplacesTheModifierStateAndKeepsOtherFields()
    {
        var command = new UniversalCommandEventArgs("save", true, true, true);
        var previous = command.Metadata;
        var replacement = new UniversalMetadata { Modifiers = UniversalModifierKeys.Windows, ClickCount = 2 };
        command.Metadata = replacement;
        Assert.That(command.Metadata, Is.SameAs(replacement));
        Assert.That(new[] { command.Ctrl, command.Alt, command.Shift }, Is.All.False);
        previous.Modifiers = UniversalModifierKeys.Control;
        Assert.That(command.Ctrl, Is.False);
        command.Shift = true;
        Assert.That(replacement.Modifiers, Is.EqualTo(UniversalModifierKeys.Windows | UniversalModifierKeys.Shift));
        Assert.That(replacement.ClickCount, Is.EqualTo(2));
    }

    [Test]
    public void Command_NullMetadataResetsToANewEmptyState()
    {
        var command = new UniversalCommandEventArgs("save", true, true, true);
        var previous = command.Metadata;
        command.Metadata = null!;
        Assert.That(command.Metadata, Is.Not.Null.And.Not.SameAs(previous));
        Assert.That(command.Metadata.Modifiers, Is.EqualTo(UniversalModifierKeys.None));
        Assert.That(new[] { command.Ctrl, command.Alt, command.Shift }, Is.All.False);
        command.Ctrl = true;
        Assert.That(command.Metadata.Modifiers, Is.EqualTo(UniversalModifierKeys.Control));
    }
}
