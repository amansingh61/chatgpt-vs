using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatGptVs.Options;

namespace ChatGptVs.Services
{
    public class ChatGptClient
    {
        private readonly HttpClient _httpClient;
        private readonly ChatGptOptionsPage _options;

        public ChatGptClient(ChatGptOptionsPage options)
        {
            _options = options;
            _httpClient = new HttpClient();
        }

        public bool HasLoginToken => !string.IsNullOrWhiteSpace(_options.LoginToken);

        public async Task<IReadOnlyList<string>> GetCompletionsAsync(string prompt, CancellationToken cancellationToken)
        {
            if (!HasLoginToken)
            {
                return new[] { "Sign in to ChatGPT from Tools > Options > ChatGPT Coding Companion." };
            }

            var payload = new
            {
                model = _options.Model,
                messages = new[] { new { role = "user", content = prompt } },
                n = _options.MaxSuggestions,
                temperature = 0.2,
                stream = false
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.LoginToken);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var completionResponse = await JsonSerializer.DeserializeAsync<ChatCompletionResponse>(
                contentStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            var completions = new List<string>();
            if (completionResponse?.Choices != null)
            {
                foreach (var choice in completionResponse.Choices)
                {
                    var content = choice?.Message?.Content;
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        completions.Add(content);
                    }
                }
            }

            return completions;
        }

        private sealed class ChatCompletionResponse
        {
            public List<ChatCompletionChoice>? Choices { get; set; }
        }

        private sealed class ChatCompletionChoice
        {
            public ChatCompletionMessage? Message { get; set; }
        }

        private sealed class ChatCompletionMessage
        {
            public string? Content { get; set; }
        }
    }
}
