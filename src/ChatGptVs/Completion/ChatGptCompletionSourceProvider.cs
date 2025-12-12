using System.ComponentModel.Composition;
using ChatGptVs.Services;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace ChatGptVs.Completion
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("ChatGPTCompletionSourceProvider")]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class ChatGptCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private readonly SuggestionSessionManager _sessionManager;

        [ImportingConstructor]
        public ChatGptCompletionSourceProvider(SuggestionSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            return new ChatGptCompletionSource(_sessionManager);
        }
    }
}
