using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ChatGptVs.Services;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

namespace ChatGptVs.Completion
{
    internal class ChatGptCompletionSource : IAsyncCompletionSource
    {
        private readonly SuggestionSessionManager _sessionManager;

        public ChatGptCompletionSource(SuggestionSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            var sourceText = triggerLocation.Snapshot.GetText();
            var suggestions = await _sessionManager.RequestSuggestionsAsync(sourceText, token).ConfigureAwait(false);

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();
            foreach (var suggestion in suggestions)
            {
                completionItems.Add(new CompletionItem(suggestion, this));
            }

            return new CompletionContext(completionItems.ToImmutable());
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            return Task.FromResult<object>($"ChatGPT suggestion: {item.DisplayText}");
        }

        public Task<CompletionChange> GetChangeAsync(IAsyncCompletionSession session, CompletionItem item, SnapshotPoint triggerLocation, CancellationToken token)
        {
            var change = new CompletionChange(item.DisplayText);
            return Task.FromResult(change);
        }
    }
}
