using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.Generic;

/// <summary>
/// Generic issue provider implementation for the <see cref="IIssueProvider"/> interface.
///
/// <para>
/// Provides a flexible, endpoint-based implementation that can be adapted to any
/// issue tracking system exposing HTTP APIs.
/// </para>
///
/// <para>
/// Supports
/// <list type="bullet">
///     <item>
///         <description>Fetching labels from a custom endpoint.</description>
///     </item>
///     <item>
///         <description>Reporting issues to a custom issue endpoint.</description>
///     </item>
/// </list>
/// </para>
///
/// <para>
/// Requires specific configuration of:
/// <list type="bullet">
///     <item><description>Labels endpoint URI</description></item>
///     <item><description>Issue endpoint URI</description></item>
///     <item><description>Label parser</description></item>
///     <item><description>Payload builder</description></item>
/// </list>
/// </para>
///
/// <example>
/// <code>
/// var provider = new GenericProvider(
///     "owner",
///     "repo",
///     "token",
///     new Uri("https://api.example.com/labels"),
///     new Uri("https://api.example.com/issues")
/// );
///
/// var labels = await provider.FetchLabels();
/// var response = await provider.Report(issue);
/// </code>
/// </example>
/// </summary>
/// <since>0.2.0</since>
public class GenericProvider : AbstractTokenProvider, IIssueProvider
{
    private static readonly HttpClient Client = new();
    
    private Uri? LabelsEndpoint { get; set; }
    private Uri? IssueEndpoint { get; set; }
    
    /// <summary>
    /// Constructs a new <see cref="GenericProvider"/> instance
    /// </summary>
    /// <param name="owner">the repository owner or organisation</param>
    /// <param name="repo">the repository name</param>
    /// <param name="token">the authentication token</param>
    /// <param name="labelsEndpoint">the endpoint used to fetch labels</param>
    /// <param name="issueEndpoint">the endpoint used to report issues</param>
    public GenericProvider(string owner, string repo, string token,
                           Uri labelsEndpoint, Uri issueEndpoint)
        : base(owner, repo, token)
    {
        LabelParser = new GenericLabelParser();
        PayloadBuilder = new GenericPayloadBuilder();
        LabelsEndpoint = labelsEndpoint;
        IssueEndpoint = issueEndpoint;
    }

    /// <summary>
    /// Fetches all labels from the configured labels endpoint
    /// </summary>
    /// <returns>a set of labels</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provider is not properly configured
    /// </exception>
    public override async Task<IReadOnlySet<Label>> FetchLabels()
    {
        EnsureConfigured();
        
        var request = new HttpRequestMessage(HttpMethod.Get, LabelsEndpoint);
        
        request.Headers.Add("Authorization", $"Bearer {Token}");
        request.Headers.Add("Accept", "application/json");

        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();
        return LabelParser!.ParseLabels(body);
    }

    /// <summary>
    /// Reports an issue to the configured issue endpoint
    /// </summary>
    /// <param name="issue">the issue to report</param>
    /// <returns>the http response</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provider is not properly configured
    /// </exception>
    public override async Task<HttpResponseMessage> Report(Issue issue)
    {
        EnsureConfigured();
        
        var payload = PayloadBuilder!.BuildPayload(issue);

        using var request = new HttpRequestMessage(HttpMethod.Post, IssueEndpoint);
        
        request.Headers.Add("Authorization", $"Bearer {Token}");
        request.Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        
        return await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
    }

    /// <summary>
    /// Ensures that the provider has been fully configured before use
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any required component or endpoint is missing
    /// </exception>
    private void EnsureConfigured()
    {
        if (LabelsEndpoint == null || IssueEndpoint == null ||
            LabelParser == null || PayloadBuilder == null)
        {
            throw new InvalidOperationException($"{nameof(GenericProvider)} must be configured before use");
        }
    }
}