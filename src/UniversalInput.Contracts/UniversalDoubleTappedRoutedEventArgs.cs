namespace UniversalInput.Contracts
{
    /// <summary>
    /// Universal class to encapsulate double tapped event information.
    /// This class is designed to mimic the behavior of DoubleTappedRoutedEventArgs from UWP.
    /// </summary>
    public class UniversalDoubleTappedRoutedEventArgs : IUniversalBase
    {
        /// <summary>
        /// Gets the location of the double tap.
        /// </summary>
        public UniversalPoint Position { get; private set; }

        /// <summary>
        /// Gets or sets a value that marks the routed event as handled. A true value for Handled prevents most handlers along the event route from handling the same event again.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the type of the device that triggered the event. This property is designed for
        /// quick access to the device information.
        /// </summary>
        public DeviceType PointerDeviceType { get; }
        /// <summary>
        /// Common data applicable to each individual event.
        /// </summary>
        public UniversalMetadata Metadata { get; set; }

        /// <summary>
        /// Legacy constructor, assuming a mouse. Use the device overload for other input sources.
        /// </summary>
        /// <param name="position">The position where the double tap occurred.</param>
        public UniversalDoubleTappedRoutedEventArgs(UniversalPoint position)
            : this(position, DeviceType.Mouse)
        {
        }

        /// <summary>Creates an event with the device identified by the platform adapter.</summary>
        public UniversalDoubleTappedRoutedEventArgs(UniversalPoint position, DeviceType deviceType)
        {
            PointerDeviceType = deviceType;
            Position = position;
            Handled = false;
        }
    }
}
