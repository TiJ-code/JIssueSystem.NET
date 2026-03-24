using JIssueSystem.NET.Api;
using JIssueSystem.NET.Core;
using JIssueSystem.NET.Provider;

namespace JIssueSystem.NET;

/// <summary>
/// Legacy wrapper for pre-0.2.0 usage.
/// Internally uses IssueReporter and GitHubProvider for 0.2.0 API.
/// </summary>
/// <since>0.1.0</since>
[Obsolete("Since 0.2.0")]
public class JIssueSystem
{
    private static readonly HttpClient Client = new();

    private readonly IssueReporter _reporter;

    /// <summary>
    /// Constructs a new instance of the reporting system
    /// </summary>
    /// <param name="repoOwner">the repository owner</param>
    /// <param name="repoName">the repository name</param>
    /// <param name="pat">then authentication token</param>
    public JIssueSystem(string repoOwner, string repoName, string pat)
    {
        _reporter = IssueReporter.NewBuilder()
            .WithProvider(IssueProviderType.GitHub, repoOwner, repoName, pat)
            .WithContract(LabelContract.DefaultContract)
            .Build();

        _reporter.InitialiseAsync().Wait();
    }
    
    /// <summary>
    /// Legacy static method, mimics old usage
    /// </summary>
    /// <param name="repoOwner">the repository owner</param>
    /// <param name="repoName">the repository name</param>
    /// <param name="pat">the personal access token or API token</param>
    /// <param name="title">the issue title</param>
    /// <param name="body">the issue description or body</param>
    /// <returns>a completable task</returns>
    [Obsolete("Since 0.2.0")]
    public static Task<int> Report(string repoOwner, string repoName, string pat, string title, string body)
    {
        return new JIssueSystem(repoOwner, repoName, pat).Report(title, body);
    }

    public async Task<int> Report(String title, String body)
    {
        var issue = new Issue
        {
            Title = title,
            Body = body,
            Labels = new() { new("automated-report") }
        };
        
        var response = await _reporter.ReportAsync(issue);

        return (int)response.StatusCode;
    }
}