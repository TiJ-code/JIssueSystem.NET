namespace JIssueSystem.NET.Api;

/// <summary>
/// Interface for building a provider-specific payload
/// to report an <see cref="Issue"/>.
/// Implementations generate the payload used to submit issues
/// to a specific provider, e.g. GitHub repository_dispatch events
/// or GitLab API requests.
/// </summary>
/// <since>0.2.0</since>
public interface IPayloadBuilder
{
    /// <summary>
    /// Builds a provider-specific payload string for a given <see cref="Issue"/>.
    /// This payload is typically a JSON string or other format required
    /// by the issue tracking API.
    /// </summary>
    /// <param name="issue">the issue to report</param>
    /// <returns>the payload string suitable for submission to the provider</returns>
    string BuildPayload(Issue issue);
}