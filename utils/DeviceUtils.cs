namespace JIssueSystem.NET.Utils;

/// <summary>
/// Utility class for retrieving device and environment information.
/// </summary>
/// <since>0.2.0</since>
public static class DeviceUtils
{
    /// <summary>
    /// Returns a formatted string containing environment diagnostics,
    /// including operating system name, architecture, and .NET runtime information.
    /// </summary>
    /// <returns>a multi-line string with environment info</returns>
    public static string GetDiagnostics()
    {
        return $@"

---
**Environment Info:**
* **OS:** {Environment.OSVersion} ({System.Runtime.InteropServices.RuntimeInformation.OSArchitecture})
* **.NET:** {Environment.Version}
";
    }
}