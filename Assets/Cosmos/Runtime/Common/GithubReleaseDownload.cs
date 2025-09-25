using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cosmos.System.Tools
{
    public static class GithubReleaseDownload
    {
        class GitHubRelease
        {
            public GitHubAsset[] assets { get; set; }
        }
        class GitHubAsset
        {
            public string url { get; set; }
            public string name { get; set; }
        }
        public static async Task Download(string owner, string repo, string fileName, string path)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("My-Awesome-App", "1.0"));
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var releases = JsonConvert.DeserializeObject<GitHubRelease>(jsonString);
            var url = releases.assets.First(a => a.name == $"{fileName}").url;

            var fileResponse = await client.GetAsync(url);
            fileResponse.EnsureSuccessStatusCode();
            using var fileStream = new FileStream(path + $"{fileName}", FileMode.Create, FileAccess.Write, FileShare.None);
            await fileResponse.Content.CopyToAsync(fileStream);
        }
    }
}
