using System.Text.Json;
using System.Text.Json.Nodes;
using JIssueSystem.NET.Api;
using JIssueSystem.NET.Provider.Generic;

namespace JIssueSystem.NET.Provider.endpoint;

/// <summary>
/// Provides authless issue reporting through a single HTTP endpoint.
/// </summary>
/// <remarks>
/// The configured URL receives a <c>GET</c> request when labels are fetched and
/// a <c>POST</c> request when an issue is reported. Label responses are expected
/// to contain JSON objects with a <c>name</c> property.
/// </remarks>
/// <param name="url">The URL used for both fetching labels and reporting issues.</param>
/// <param name="customPayloadBuilder">
/// An optional builder for report payloads. When omitted, the payload contains
/// <c>title</c>, <c>body</c>, and <c>labels</c> properties.
/// </param>
/// <example>
/// <code>
/// var provider = new EndpointProvider("https://example.com/issues");
/// var labels = await provider.FetchLabels();
/// var response = await provider.Report(issue);
/// </code>
/// </example>
/// <since>0.4.0</since>
public class EndpointProvider(string url, IPayloadBuilder? customPayloadBuilder = null) : IIssueProvider
{
    private static readonly HttpClient Client = new();

    /// <summary>
    /// Fetches labels by sending an authless <c>GET</c> request to the configured URL.
    /// </summary>
    /// <returns>The unique labels parsed from <c>name</c> properties in the response.</returns>
    /// <exception cref="HttpRequestException">
    /// The request failed or the endpoint returned an unsuccessful status code.
    /// </exception>
    public async Task<IReadOnlySet<Label>> FetchLabels()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "JIssueSystem");

        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        return new GenericLabelParser().ParseLabels(body);
    }

    /// <summary>
    /// Reports an issue by sending an authless JSON <c>POST</c> request to the configured URL.
    /// </summary>
    /// <param name="issue">The issue to report.</param>
    /// <returns>The response returned by the endpoint.</returns>
    public Task<HttpResponseMessage> Report(Issue issue)
    {
        return Report(issue, null);
    }

    /// <summary>
    /// Reports an issue with additional top-level payload properties.
    /// </summary>
    /// <param name="issue">The issue to report.</param>
    /// <param name="optionalProperties">
    /// Additional string properties to merge into the JSON payload. Existing properties
    /// with the same names are replaced.
    /// </param>
    /// <returns>The response returned by the endpoint.</returns>
    /// <exception cref="JsonException">The generated payload is not valid JSON.</exception>
    /// <exception cref="InvalidOperationException">The generated payload is not a JSON object.</exception>
    public async Task<HttpResponseMessage> Report(Issue issue, Dictionary<string, string>? optionalProperties)
    {
        string payload = customPayloadBuilder?.BuildPayload(issue) ?? JsonSerializer.Serialize(new
        {
            title = issue.Title,
            body = issue.Body,
            labels = issue.Labels.Select(l => l.Name).ToArray(),
        });

        if (optionalProperties is not null && optionalProperties.Count > 0)
        {
            var json = JsonObject.Parse(payload).AsObject();
            foreach (var property in optionalProperties)
            {
                json[property.Key] = property.Value;
            }
            
            payload = JsonSerializer.Serialize(json);
        }
        
        
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json")
        };
        
        request.Headers.Add("User-Agent", "JIssueSystem");

        return await Client.SendAsync(request);
    }
}
