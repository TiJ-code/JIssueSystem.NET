namespace JIssueSystem.NET.Api;

/// <summary>
/// Represents an issue to report to a tracking system.
/// </summary>
/// <since>0.2.0</since>
public record Issue
{
    /// <summary>
    /// Title of the issue
    /// </summary>
    public string Title { get; init; } = "";
    
    /// <summary>
    /// Body of the issue
    /// </summary>
    public string Body { get; init; } = "";
    
    /// <summary>
    /// Labels associated with the issue
    /// </summary>
    public HashSet<Label> Labels { get; init; } = new();
}