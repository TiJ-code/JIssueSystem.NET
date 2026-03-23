using System.Text;
using System.Text.Json;

namespace JIssueSystem.NET;

public class JIssueSystem(string repoOwner, string repoName, string pat)
{
    private static readonly HttpClient Client = new();
    
    public static Task<int> Report(string repoOwner, string repoName, string pat, string title, string body)
    {
        return new JIssueSystem(repoOwner, repoName, pat).Report(title, body);
    }

    public async Task<int> Report(String title, String body)
    {
        var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/dispatches";

        var payload = new
        {
            event_type = "create-issue",
            client_payload = new
            {
                title = title,
                body = body + GetDiagnostics()
            }
        };

        string jsonPayload = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {pat.Trim()}");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Headers.Add("User-Agent", $"JIssueSystem-Library");

        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await Client.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine(response);

        if ((int)response.StatusCode >= 400)
        {
            Console.Error.WriteLine("GitHub Message: " + responseBody);
        }

        return (int)response.StatusCode;
    }

    private static string GetDiagnostics()
    {
        return $@"

---
**Environment Info:**
* **OS:** {Environment.OSVersion} ({System.Runtime.InteropServices.RuntimeInformation.OSArchitecture})
* **.NET:** {Environment.Version}
";
    }
}