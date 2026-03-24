namespace JIssueSystem.NET.Api;

/// <summary>
/// Interface for parsing labels from a raw string input.
/// Implementations are responsible for converting arbitrary string
/// representations into a set of <see cref="Label"/> objects.
/// This is typically used when receiving labels from an API response
/// or user input that needs to be transformed into strongly-typed
/// <see cref="Label"/> objects.
/// </summary>
/// <since>0.2.0</since>
public interface ILabelParser
{
    /// <summary>
    /// Parses a raw string of labels into a set of <see cref="Label"/> objects.
    /// Implementations may split the string, remove whitespaces,
    /// and create <see cref="Label"/> instances accordingly.
    /// </summary>
    /// <param name="rawLabels">The raw string containing label information, e.g. "bug, enhancement"</param>
    /// <returns>A set of <see cref="Label"/> objects representing the parsed labels</returns>
    IReadOnlySet<Label> ParseLabels(string rawLabels);
}