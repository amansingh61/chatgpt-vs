using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using ChatGptVs.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace ChatGptVs.Commands
{
    internal sealed class ApplySuggestionCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("172c7691-47b3-43d2-81fd-323243a2825e");

        private readonly AsyncPackage _package;
        private readonly SuggestionSessionManager _suggestionSessionManager;

        private ApplySuggestionCommand(AsyncPackage package, SuggestionSessionManager suggestionSessionManager, OleMenuCommandService commandService)
        {
            _package = package;
            _suggestionSessionManager = suggestionSessionManager;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package, SuggestionSessionManager suggestionSessionManager)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            _ = new ApplySuggestionCommand(package, suggestionSessionManager, commandService!);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var textView = await _package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager2;
                if (textView == null)
                {
                    return;
                }

                var activeView = GetActiveView(textView);
                if (activeView == null)
                {
                    return;
                }

                var selection = GetSelectedText(activeView);
                var suggestions = await _suggestionSessionManager.RequestSuggestionsAsync(selection, _package.DisposalToken);
                var nextSuggestion = suggestions.Count > 0 ? suggestions[0] : string.Empty;

                if (string.IsNullOrWhiteSpace(nextSuggestion))
                {
                    ShowMessageBox("ChatGPT Coding Companion", "No suggestions returned. Please verify your ChatGPT login token.");
                    return;
                }

                var shouldReplace = ShouldReplaceWithConfirmation(nextSuggestion);
                if (shouldReplace)
                {
                    ReplaceSelection(activeView, nextSuggestion);
                }
            });
        }

        private bool ShouldReplaceWithConfirmation(string suggestion)
        {
            const string prompt = "Apply the following ChatGPT suggestion?\n\n";
            var result = VsShellUtilities.ShowMessageBox(
                _package,
                prompt + suggestion,
                "ChatGPT Replacement",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            return result == (int)VSConstants.MessageBoxResult.IDYES;
        }

        private static void ReplaceSelection(IVsTextView view, string replacement)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            view.GetSelection(out var startLine, out var startColumn, out var endLine, out var endColumn);
            view.GetBuffer(out var textLines);
            textLines.ReplaceLines(startLine, startColumn, endLine, endColumn, replacement, replacement.Length, out _);
        }

        private static string GetSelectedText(IVsTextView view)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            view.GetSelection(out var startLine, out var startColumn, out var endLine, out var endColumn);
            view.GetBuffer(out var lines);
            lines.GetLineText(startLine, startColumn, endLine, endColumn, out var selectedText);
            return selectedText;
        }

        private static IVsTextView? GetActiveView(IVsTextManager2 textManager)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out var activeView);
            return activeView;
        }

        private void ShowMessageBox(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                _package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
