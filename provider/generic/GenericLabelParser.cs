using System.Text.RegularExpressions;
using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.Generic;

/// <summary>
/// Default label parser implementation for generic issue providers.
///
/// <para>
/// Extracts label names from raw JSON responses using a regular expression.
/// Designed for simple or loosely structured APIs where a full JSON parser
/// is not required.
/// </para>
///
/// <para>
/// Supports
/// <list type="bullet">
///     <item>
///         <description>Custom parsing logic via a user-defined <see cref="Func{T, TResult}"/>.</description>
///     </item>
///     <item>
///         <description>Default extraction of <c>"name"</c> fields from JSON.</description>
///     </item>
/// </list>
/// </para>
///
/// <example>
/// <code>
/// var parser = new GenericLabelParser();
///
/// // Default parsing
/// var labels = parser.ParseLabels(json);
///
/// // Custom parser
/// parser.Parser = raw => new HashSet&lt;Label&gt; {
///     new Label("custom")
/// };
/// </code>
/// </example>
/// </summary>
/// <since>0.2.0</since>
public class GenericLabelParser : ILabelParser
{
    private static readonly Regex GenericJsonLabelPattern = new("\"name\"\\s*:\\s*\"([^\"]+)\"", RegexOptions.Compiled);

    /// <summary>
    /// Gets or sets the label parsing function
    /// </summary>
    public Func<string, HashSet<Label>> Parser { get; set; } = DefaultParser;

    /// <summary>
    /// Parses labels from a raw response string
    /// </summary>
    /// <param name="rawLabels">the raw label response</param>
    /// <returns>a set of parsed labels</returns>
    public IReadOnlySet<Label> ParseLabels(string rawLabels)
    {
        return Parser(rawLabels);
    }

    /// <summary>
    /// Default label parsing implementation
    /// </summary>
    /// <param name="rawLabels">the raw JSON string</param>
    /// <returns>a set of extracted labels</returns>
    private static HashSet<Label> DefaultParser(String rawLabels)
    {
        if (string.IsNullOrWhiteSpace(rawLabels))
            return new HashSet<Label>();
        
        var labels = new HashSet<Label>();
        var matches = GenericJsonLabelPattern.Matches(rawLabels);

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
                labels.Add(new Label(match.Groups[1].Value));
        }

        return labels;
    }
}