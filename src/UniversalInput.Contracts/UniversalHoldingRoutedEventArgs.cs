namespace UniversalInput.Contracts
{
    /// <summary>
    /// Universal class to encapsulate holding event information.
    /// This class is designed to mimic the behavior of HoldingRoutedEventArgs from UWP.
    /// </summary>
    public class UniversalHoldingRoutedEventArgs : IUniversalBase
    {
        /// <summary>
        /// Gets the location of the holding event.
        /// </summary>
        public UniversalPoint Position { get; private set; }

        /// <summary>
        /// Gets or sets the holding state (e.g., started, completed, canceled).
        /// </summary>
        public UniversalHoldingState HoldingState { get; private set; }

        /// <summary>
        /// Gets or sets a value that marks the routed event as handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the type of the device that triggered the event. This property is designed for
        /// quick access to the device information.
        /// </summary>
        public DeviceType PointerDeviceType { get; }

        /// <summary>
        /// Legacy constructor, assuming a mouse. Use the device overload for other input sources.
        /// </summary>
        /// <param name="position">The position where the holding gesture occurred.</param>
        /// <param name="holdingState">The state of the holding gesture.</param>
        public UniversalHoldingRoutedEventArgs(UniversalPoint position, UniversalHoldingState holdingState)
            : this(position, holdingState, DeviceType.Mouse)
        {
        }

        /// <summary>Creates an event with the device identified by the platform adapter.</summary>
        public UniversalHoldingRoutedEventArgs(UniversalPoint position, UniversalHoldingState holdingState, DeviceType deviceType)
        {
            PointerDeviceType = deviceType;
            Position = position;
            HoldingState = holdingState;
            Handled = false;
        }

        // Additional properties and methods as necessary

        /// <summary>
        /// Common data applicable to each individual event.
        /// </summary>
        public UniversalMetadata Metadata { get; set; }
    }
}
