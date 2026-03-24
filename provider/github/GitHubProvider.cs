using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Provider.GitHub;

/// <summary>
/// GitHub issue provider implementation for the <see cref="IIssueProvider"/> interface.
///
/// <para>
/// Supports
/// <list type="bullet">
///     <item>
///         <description>Fetching all labels of a repository via <c>/labels</c> endpoint.</description>
///     </item>
///     <item>
///         <description>Reporting issue via the <c>repository_dispatch</c> event</description>
///     </item>
/// </list>
/// </para>
///
/// <example>
/// <code>
/// var provider = new GitHubProvider("owner", "repo", "PAT");
/// var labels = await provider.FetchLabels();
/// var response = await provider.Report(issue);
/// </code>
/// </example>
/// </summary>
/// <since>0.2.0</since>
public class GitHubProvider : AbstractTokenProvider, IIssueProvider
{
    private static readonly HttpClient Client = new();
    
    /// <summary>
    /// Constructs a new <see cref="GitHubProvider"/> instance
    /// </summary>
    /// <param name="owner">the repository owner or organisation</param>
    /// <param name="repo">the repository name</param>
    /// <param name="token">the authentication token</param>
    public GitHubProvider(string owner, string repo, string token) : base(owner, repo, token)
    {
        LabelParser = GitHubLabelParser.Instance;
        PayloadBuilder = GitHubPayloadBuilder.Instance;
    }

    /// <summary>
    /// Fetches all labels of the repository
    /// </summary>
    /// <returns>a set of labels</returns>
    /// <exception cref="NotSupportedException">Thrown if there is no
    /// <see cref="ILabelParser"/> is configured</exception>
    public override async Task<IReadOnlySet<Label>> FetchLabels()
    {
        var url = $"https://api.github.com/repos/{Owner}/{Repo}/labels";
        
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {Token}");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("User-Agent", "JIssueSystemNET");
        
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var jsonBody = await response.Content.ReadAsStringAsync();
        
        if (LabelParser == null)
            throw new NotSupportedException("LabelParser not set");
        
        return new HashSet<Label>(LabelParser.ParseLabels(jsonBody));
    }

    /// <summary>
    /// Reports an issue to the repository via the <c>repository_dispatch</c> event
    /// </summary>
    /// <param name="issue">the issue to report</param>
    /// <returns>the http response</returns>
    /// <exception cref="NotSupportedException"></exception>
    public override async Task<HttpResponseMessage> Report(Issue issue)
    {
        var url = $"https://api.github.com/repos/{Owner}/{Repo}/dispatches";
        
        if (PayloadBuilder == null)
            throw new NotSupportedException("PayloadBuilder not set");
        
        var payload = PayloadBuilder.BuildPayload(issue);

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json")
        };
        
        request.Headers.Add("Authorization", $"Bearer {Token}");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("User-Agent", "JIssueSystem");

        return await Client.SendAsync(request);
    }
}