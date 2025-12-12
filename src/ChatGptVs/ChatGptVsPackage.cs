using System;
using System.Runtime.InteropServices;
using System.Threading;
using ChatGptVs.Commands;
using ChatGptVs.Options;
using ChatGptVs.Services;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ChatGptVs
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("ChatGPT Coding Companion", "Copilot-style coding assistance powered by ChatGPT login and inference", "0.1")]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(ChatGptOptionsPage), "ChatGPT Coding Companion", "General", 0, 0, true)]
    public sealed class ChatGptVsPackage : AsyncPackage
    {
        public const string PackageGuidString = "39cb2b37-e7ad-4be2-a638-17e64e666d8f";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var settings = (ChatGptOptionsPage)await GetDialogPageAsync(typeof(ChatGptOptionsPage));
            var chatGptClient = new ChatGptClient(settings);
            var suggestionManager = new SuggestionSessionManager(chatGptClient);

            await ApplySuggestionCommand.InitializeAsync(this, suggestionManager);
        }
    }
}
