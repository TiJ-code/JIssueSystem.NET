namespace JIssueSystem.NET.Api;

/// <summary>
/// Abstract base class for token-based <see cref="IIssueProvider"/> implementations.
///
/// <para>
/// Stores repository context and authentication token for subclasses. Provides
/// default implementations for managing payload builders and label parses.
/// </para>
/// </summary>
/// <since>0.2.0</since>
public abstract class AbstractTokenProvider : IIssueProvider
{
    protected readonly string Owner, Repo, Token;
    
    /// <summary>
    /// Builder used to construct API request payloads
    /// </summary>
    public IPayloadBuilder? PayloadBuilder { get; set; }
    
    /// <summary>
    /// Parser used to extract labels from API responses
    /// </summary>
    public ILabelParser? LabelParser { get; set; }

    protected AbstractTokenProvider(string owner, string repo, string token)
    {
        if (string.IsNullOrEmpty(owner))
            throw new ArgumentException("Owner cannot be null or empty", nameof(owner));
        if (string.IsNullOrEmpty(repo))
            throw new ArgumentException("Repo cannot be null or empty", nameof(repo));
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        
        Owner = owner;
        Repo = repo;
        Token = token;
    }
    
    public abstract Task<IReadOnlySet<Label>> FetchLabels();

    public abstract Task<HttpResponseMessage> Report(Issue issue);
}