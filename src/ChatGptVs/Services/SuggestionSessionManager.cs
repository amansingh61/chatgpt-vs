using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatGptVs.Services
{
    public class SuggestionSessionManager
    {
        private readonly ChatGptClient _client;

        public SuggestionSessionManager(ChatGptClient client)
        {
            _client = client;
        }

        public Task<IReadOnlyList<string>> RequestSuggestionsAsync(string sourceText, CancellationToken cancellationToken)
        {
            return _client.GetCompletionsAsync(sourceText, cancellationToken);
        }
    }
}
