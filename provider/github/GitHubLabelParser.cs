using System.Text.RegularExpressions;
using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.GitHub;

/// <summary>
/// Utility class for parsing GitHub label JSON responses
///
/// <para>
/// Extracts label names from GitHub API responses and converts them into
/// <see cref="Label"/> objects.
/// </para>
/// <example>
/// <code>
/// var labels = GitHubLabelParser.Instance.ParseLabels(rawJson);
/// </code>
/// </example>
/// </summary>
public class GitHubLabelParser : ILabelParser
{
    /// <summary>
    /// The singleton instance
    /// </summary>
    public static readonly GitHubLabelParser Instance = new();

    private static readonly Regex NamePattern = new("\"name\"\\s*:\\s*\"([^\"]+)\"", RegexOptions.Compiled);
    
    private GitHubLabelParser() {}
    
    /// <summary>
    /// Parses a JSON string of GitHub labels.
    /// </summary>
    /// <param name="rawLabels">The raw JSON string to be parsed</param>
    /// <returns>a set of parsed labels</returns>
    public IReadOnlySet<Label> ParseLabels(string rawLabels)
    {
        if (string.IsNullOrWhiteSpace(rawLabels))
            return new HashSet<Label>();
        
        var labels = new HashSet<Label>();
        var matches = NamePattern.Matches(rawLabels);

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
                labels.Add(new Label(match.Groups[1].Value));
        }

        return labels;
    }
}