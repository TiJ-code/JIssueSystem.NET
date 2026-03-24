using System.Text.Json;
using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.Generic;

/// <summary>
/// Default payload builder implementation for generic issue providers.
///
/// <para>
/// Converts an <see cref="Issue"/> into a JSON payload suitable for submission
/// to a remote issue tracking API.
/// </para>
///
/// <para>
/// Supports
/// <list type="bullet">
///     <item>
///         <description>Custom payload generation via a user-defined <see cref="Func{T, TResult}"/>.</description>
///     </item>
///     <item>
///         <description>Default JSON serialization including <c>title</c> and <c>body</c>.</description>
///     </item>
/// </list>
/// </para>
///
/// <example>
/// <code>
/// var builder = new GenericPayloadBuilder();
///
/// // Use default behaviour
/// var payload = builder.BuildPayload(issue);
///
/// // Custom payload
/// builder.Builder = issue => JsonSerializer.Serialize(new {
///     summary = issue.Title,
///     description = issue.Body
/// });
/// </code>
/// </example>
/// </summary>
/// <since>0.2.0</since>
public class GenericPayloadBuilder : IPayloadBuilder
{
    /// <summary>
    /// Gets or sets the payload builder function
    /// </summary>
    public Func<Issue, string> Builder { get; set; } = DefaultBuilder;
    
    /// <summary>
    /// Builds the payload for the given issue
    /// </summary>
    /// <param name="issue">the issue to serialize</param>
    /// <returns>a JSON payload string</returns>
    public string BuildPayload(Issue issue)
    {
        return Builder(issue);
    }

    /// <summary>
    /// Default payload builder implementation
    /// </summary>
    /// <param name="issue">the issue to serialize</param>
    /// <returns>a JSON string containing title and body</returns>
    private static string DefaultBuilder(Issue issue)
    {
        return JsonSerializer.Serialize(new
            {
                title = issue.Title,
                body = issue.Body
            }
        );
    }
}