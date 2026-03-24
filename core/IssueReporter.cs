using JIssueSystem.NET.Api;
using JIssueSystem.NET.Provider;
using JIssueSystem.NET.Utils;

namespace JIssueSystem.NET.Core;

/// <summary>
/// High-level usability for reporting issues to a repository via an <see cref="IIssueProvider"/>.
///
/// <para>
/// Manages initialisation, label validation, and automatic enrichment of issue
/// bodies with diagnostic information.
/// </para>
///
/// <para>
/// Supports a fluent <see cref="Builder"/> for convenient construction.
/// </para>
///
/// <example>
/// <code>
/// var reporter = IssueReporter.NewBuilder()
///     .WithProvider(IssueProviderType.GitHub, "owner", "repo", "token")
///     .WithContract(LabelContract.DefaultContract)
///     .Build();
/// </code>
/// </example>
/// </summary>
/// <since>0.2.0</since>
public class IssueReporter
{
    private readonly IIssueProvider _provider;
    private readonly LabelContract _contract;
    private volatile bool _initialised;

    /// <summary>
    /// Creates a new see <see cref="IssueReporter"/>
    /// </summary>
    /// <param name="provider">the issue provider.</param>
    /// <param name="contract">the label contract to enforce</param>
    public IssueReporter(IIssueProvider provider, LabelContract contract)
    {
        _provider = provider;
        _contract = contract;
    }

    /// <summary>
    /// Initialises the reporter by fetching repository labels and validating them
    /// </summary>
    public async Task InitialiseAsync()
    {
        var labels = await _provider.FetchLabels();
        _contract.Validate(labels);
        _initialised = true;
    }

    /// <summary>
    /// Reports an issue after enriching it with diagnostics and validating them.
    /// </summary>
    /// <param name="issue">the issue to report</param>
    /// <returns>The HTTP response message from the provider.</returns>
    /// <exception cref="InvalidOperationException">Thrown if reporter is not initialised</exception>
    public async Task<HttpResponseMessage> ReportAsync(Issue issue)
    {
        if (!_initialised)
            throw new InvalidOperationException("Reporter not initialised");

        var enriched = Enrich(issue);
        _contract.Validate(enriched.Labels);
        
        return await _provider.Report(enriched);
    }
    
    /// <summary>
    /// Gets the underlying issue provider.
    /// </summary>
    public IIssueProvider Provider => _provider;
    
    /// <summary>
    /// Gets the label contract.
    /// </summary>
    public LabelContract Contract => _contract;
    
    private Issue Enrich(Issue issue)
    {
        var enrichedLabels = new HashSet<Label>(issue.Labels);
        enrichedLabels.UnionWith(_contract.RequiredLabels.Select(name => new Label(name)));

        return new Issue
        {
            Title = issue.Title,
            Body = $"{issue.Body}\n{DeviceUtils.GetDiagnostics()}",
            Labels = enrichedLabels,
        };
    }

    /// <summary>
    /// Returns a new builder for <see cref="IssueReporter"/>.
    /// </summary>
    /// <returns>A builder instance</returns>
    public static Builder NewBuilder() => new();

    /// <summary>
    /// Builder class for constructing <see cref="IssueReporter"/> instances
    /// </summary>
    public class Builder
    {
        private IIssueProvider? _provider;
        private LabelContract? _contract;

        /// <summary>
        /// Sets the provider directly.
        /// </summary>
        /// <param name="provider">Provider instance</param>
        /// <returns>this instance</returns>
        public Builder WithProvider(IIssueProvider provider)
        {
            _provider = provider;
            return this;
        }

        /// <summary>
        /// Sets the provider via parameters.
        /// </summary>
        /// <param name="type">the issue provider type</param>
        /// <param name="owner">the repository owner</param>
        /// <param name="repo">the repository name</param>
        /// <param name="token">the authentication token</param>
        /// <returns>this instance</returns>
        public Builder WithProvider(IssueProviderType type, string owner, string repo, string token)
        {
            _provider = IssueProviderFactory.Create(type, owner, repo, token);
            return this;
        }

        /// <summary>
        /// Sets the label contract.
        /// </summary>
        /// <param name="contract">the contract labels have to follow</param>
        /// <returns>this instance</returns>
        public Builder WithContract(LabelContract contract)
        {
            _contract = contract;
            return this;
        }

        /// <summary>
        /// Constructs the <see cref="IssueReporter"/> instance.
        /// </summary>
        /// <returns>the <see cref="IssueReporter"/> instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if no provider configured</exception>
        public IssueReporter Build()
        {
            if (_provider == null)
                throw new InvalidOperationException("Provider must be set");
            
            _contract ??= LabelContract.DefaultContract;
            
            return new IssueReporter(_provider, _contract);
        }
    }

    /// <summary>
    /// Factory for creating <see cref="IIssueProvider"/> instances
    /// </summary>
    private static class IssueProviderFactory
    {
        public static IIssueProvider Create(IssueProviderType type, string owner, string repo, string token)
        {
            if (type == IssueProviderType.None)
                throw new NotSupportedException("Cannot create an issue provider");
            return type.Creator!(owner, repo, token);
        }
    }
}