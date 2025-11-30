using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using System;

public static class HardcoreGarbageCollector
{
    // Menu item: Tools  Memory  Hardcore GC
    // Hotkey example: Ctrl Shift G (Cmd Shift G on macOS)
    [MenuItem("Tools/Memory/Hardcore GC %#g")]
    private static void RunHardcoreGC()
    {
        // Get memory before
        long before = Profiler.GetTotalAllocatedMemoryLong();

        // Unload unused assets in the editor
        EditorUtility.UnloadUnusedAssetsImmediate();

        // Aggressive GC sequence
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Get memory after
        long after = Profiler.GetTotalAllocatedMemoryLong();
        long diffBytes = before - after;
        float diffMB = diffBytes / (1024f * 1024f);

        Debug.Log($"[Hardcore GC] Freed approximately {diffMB:F1} MB");
    }
}