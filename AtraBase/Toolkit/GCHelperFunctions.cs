using System.Runtime;

namespace AtraBase.Toolkit;

/// <summary>
/// Helper functions for GC.
/// </summary>
internal static class GCHelperFunctions
{
    /// <summary>
    /// Asks for a full, compacting GC.
    /// </summary>
    internal static void RequestFullGC()
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
    }
}
