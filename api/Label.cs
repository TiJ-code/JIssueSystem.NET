namespace JIssueSystem.NET.Api;

/// <summary>
/// Represents a label associated with an <see cref="Issue"/>.
/// <para>
/// This is an immutable value type that holds the name of a label.
/// Labels are typically used to categorise or tag issues in an issue tracking system,
/// for example "bug", "enhancement", or "automated-report".
/// </para>
/// <para>Example usage:</para>
/// <code>
/// var bugLabel = new Label("bug");
/// var automatedLabel = new Label("automated-report");
/// </code>
/// </summary>
/// <param name="Name">The name of the label</param>
/// <since>0.2.0</since>
public record Label(string Name);