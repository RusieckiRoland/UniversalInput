using System.Collections.Generic;

namespace UniversalInput.Contracts
{
    /// <summary>Generic editor command (e.g., "save", "formatDocument").</summary>
    public sealed class UniversalCommandEventArgs : IUniversalBase
    {
        private UniversalMetadata metadata = new UniversalMetadata();
        /// <summary>Identifier like "ctrl+s", "formatDocument", "goToDefinition".</summary>
        public string CommandId { get; set; } = string.Empty;

        /// <summary>Keyboard modifiers for shortcut-originated commands.</summary>
        public bool Ctrl
        {
            get => HasModifier(UniversalModifierKeys.Control);
            set => SetModifier(UniversalModifierKeys.Control, value);
        }
        public bool Alt
        {
            get => HasModifier(UniversalModifierKeys.Alt);
            set => SetModifier(UniversalModifierKeys.Alt, value);
        }
        public bool Shift
        {
            get => HasModifier(UniversalModifierKeys.Shift);
            set => SetModifier(UniversalModifierKeys.Shift, value);
        }

        /// <summary>String arguments associated with the command.</summary>
        public IDictionary<string, string> Arguments { get; } = new Dictionary<string, string>();

        public object Parameter { get; set; }

        public DeviceType PointerDeviceType { get; private set; } = DeviceType.Other;
        /// <summary>
        /// Owns the modifier state. Replacing metadata replaces that state;
        /// assigning null resets it to a new empty metadata object.
        /// </summary>
        public UniversalMetadata Metadata
        {
            get => metadata;
            set => metadata = value ?? new UniversalMetadata();
        }

        private bool HasModifier(UniversalModifierKeys flag) => (metadata.Modifiers & flag) != 0;

        private void SetModifier(UniversalModifierKeys flag, bool enabled)
        {
            metadata.Modifiers = enabled ? metadata.Modifiers | flag : metadata.Modifiers & ~flag;
        }

        public UniversalCommandEventArgs(string commandId, bool ctrl, bool alt, bool shift)
        {
            CommandId = commandId ?? string.Empty;
            Ctrl = ctrl; Alt = alt; Shift = shift;
        }
    }
}
