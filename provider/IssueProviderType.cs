using JIssueSystem.NET.Api;
using JIssueSystem.NET.Provider.endpoint;
using JIssueSystem.NET.Provider.GitHub;

namespace JIssueSystem.NET.Provider;

/// <summary>
/// Enumerates the supported issue provider types
///
/// <para>
/// Each type optionally supplies a <see cref="ProviderCreator"/> used to
/// instantiate the corresponding <see cref="IIssueProvider"/> implementation.
/// </para>
///
/// <para>
/// This class is primarily used by <see cref="JIssueSystem.NET.Core.IssueReporter.Builder"/>
/// to create provider instances without exposing implementation class.
/// </para>
/// </summary>
/// <since>0.2.0</since>
public sealed class IssueProviderType
{
    /// <summary>
    /// Invalid issue provider type.
    /// </summary>
    public static readonly IssueProviderType None = 
        new(null);

    /// <summary>
    /// GitHub issue provider.
    /// </summary>
    public static readonly IssueProviderType GitHub =
        new((owner, repo, token) => new GitHubProvider(owner, repo, token));

    /// <summary>
    /// Endpoint issue provider builder function.
    /// </summary>
    /// <param name="url">The URL used to fetch labels and report issues.</param>
    /// <param name="customPayloadBuilder">An optional custom issue payload builder.</param>
    /// <param name="requestHeaders">Optional headers to send with both requests.</param>
    /// <param name="labelQueryParameters">Optional query parameters sent when fetching labels.</param>
    /// <param name="reportPayloadProperties">Optional properties added to every report payload.</param>
    public static EndpointProvider CreateEndpointProvider(
        string url,
        IPayloadBuilder? customPayloadBuilder = null,
        IReadOnlyDictionary<string, string>? requestHeaders = null,
        IReadOnlyDictionary<string, string>? labelQueryParameters = null,
        IReadOnlyDictionary<string, string>? reportPayloadProperties = null) =>
        new(url, customPayloadBuilder, requestHeaders, labelQueryParameters, reportPayloadProperties);
    
    
    /// <summary>
    /// Factory used to create the provider instance for this type.
    /// </summary>
    public ProviderCreator? Creator { get; init; }
    
    private IssueProviderType(ProviderCreator? creator)
    {
        Creator = creator;
    }

    /// <summary>
    /// Delegate used to construct <see cref="IIssueProvider"/> instances.
    /// </summary>
    /// <param name="owner">Repository owner or organisation.</param>
    /// <param name="repo">Repository name.</param>
    /// <param name="token">Authentication token.</param>
    /// <returns>A configured <see cref="IIssueProvider"/>.</returns>
    public delegate IIssueProvider ProviderCreator(string owner, string repo, string token);
}
