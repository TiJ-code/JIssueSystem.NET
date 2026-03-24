using JIssueSystem.NET.Api;

namespace JIssueSystem.NET.Core;

/// <summary>
/// Represents a contract of required labels for a repository.
///
/// <para>
/// Validating that certain labels exist and follows checking if
/// a label is allowed or merging with other contracts
/// </para>
/// </summary>
/// <since>0.2.0</since>
public class LabelContract
{
    /// <summary>
    /// Default contract including "automated-report" label
    /// </summary>
    public static readonly LabelContract DefaultContract = new(new HashSet<string> { "automated-report" });
    
    /// <summary>
    /// GitHub default labl
    /// </summary>
    public static readonly LabelContract GitHubContract = DefaultContract.Concat(new(
        new HashSet<string> { "enhancement", "bug", "duplicate" })
    );
    
    private readonly HashSet<string> _requiredLabels;

    /// <summary>
    /// Creates a new <see cref="LabelContract"/> instance with a given list of labels
    /// </summary>
    /// <param name="requiredLabels"></param>
    public LabelContract(IEnumerable<string> requiredLabels)
    {
        _requiredLabels = new HashSet<string>(requiredLabels);
    }
    
    /// <summary>
    /// Accessor of required labels
    /// </summary>
    public IReadOnlySet<string> RequiredLabels => _requiredLabels;

    /// <summary>
    /// Validates that all required labels exist in the given repository.
    /// </summary>
    /// <param name="repoLabels">the set of labels in the repository</param>
    /// <exception cref="InvalidOperationException">Throws if any required label is missing</exception>
    public void Validate(IEnumerable<Label> repoLabels)
    {
        var existing = new HashSet<string>(repoLabels.Select(l => l.Name));
        var missing = _requiredLabels.Except(existing).ToList();
        
        if (missing.Any())
            throw new InvalidOperationException($"Repository missing required labels: {string.Join(", ", missing)}");
    }

    /// <summary>
    /// Merges the contract with another, producing a new contract object.
    /// </summary>
    /// <param name="other">the other contract</param>
    /// <returns>a new <see cref="LabelContract"/> containing all labels from both</returns>
    public LabelContract Concat(LabelContract other)
    {
        var combined = new HashSet<string>(_requiredLabels);
        combined.UnionWith(other._requiredLabels);
        return new LabelContract(combined);
    }

    /// <summary>
    /// Checks whether a given label is allowed according to this contract.
    /// </summary>
    /// <param name="label">the label to check</param>
    /// <returns>true if allowed</returns>
    public bool IsAllowed(Label label)
    {
        return _requiredLabels.Contains(label.Name);
    }
}