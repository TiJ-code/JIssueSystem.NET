namespace JIssueSystem.NET.Api;

/// <summary>
/// Interface representing a provider for issue tracking systems.
///
/// <para>
/// Implementations of this interface should handle fetching labels from a
/// repository and reporting new issues.
/// </para>
/// </summary>
/// <since>0.2.0</since>
public interface IIssueProvider
{
    /// <summary>
    /// Fetches the set of labels available in the repository
    /// </summary>
    /// <returns>a set of <see cref="Label"/> objects</returns>
    Task<IReadOnlySet<Label>> FetchLabels();

    /// <summary>
    /// Reports a new issue to a provider.
    /// </summary>
    /// <param name="issue">the issue to report</param>
    /// <returns>the response message from the provider</returns>
    Task<HttpResponseMessage> Report(Issue issue);
}