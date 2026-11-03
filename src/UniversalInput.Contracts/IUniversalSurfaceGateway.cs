using System;

namespace UniversalInput.Contracts
{
    public interface IUniversalSurfaceGateway<out TSnapshot>
        where TSnapshot : IUniversalSurfaceSnapshot
    {
        event EventHandler SnapshotChanged;

        event EventHandler FocusRequested;

        TSnapshot CurrentSnapshot { get; }
    }
}
