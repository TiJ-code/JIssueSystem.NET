using System.Text.Json;
using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.GitHub;

/// <summary>
/// Builds JSON payloads for creating GitHub Issues via the <c>repository_dispatch</c> event.
///
/// <para>
/// This class is a singleton.
/// </para>
///
/// <para>
/// The payload includes:
/// <list type="bullet">
///     <item>Issue title</item>
///     <item>Issue body</item>
///     <item>Labels as JSON array</item>
/// </list>
/// </para>
/// </summary>
/// <since>0.2.0</since>
public class GitHubPayloadBuilder : IPayloadBuilder
{
    /// <summary>
    /// The singleton instance
    /// </summary>
    public static readonly GitHubPayloadBuilder Instance = new();

    private const string JsonTemplate = """
    {{
        "event_type": "create-issue",
        "client_payload": {{
            "title": "{0}",
            "body": "{1}",
            "labels": [{2}]
        }}
    }}
    """;
    
    private GitHubPayloadBuilder() {}

    /// <summary>
    /// Builds a JSON payload for the given issue.
    /// </summary>
    /// <param name="issue">the issue to build a payload for</param>
    /// <returns>a JSON string representing the issue</returns>
    public string BuildPayload(Issue issue)
    {
        string title = JsonEncodedText.Encode(issue.Title).ToString();
        string body = JsonEncodedText.Encode(issue.Body).ToString();

        string labels = string.Join(",",
            issue.Labels.Select(l => $"{JsonEncodedText.Encode(l.Name)}")
        );
        
        return string.Format(JsonTemplate, title, body, labels);
    }
}