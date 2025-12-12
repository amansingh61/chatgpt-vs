using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ChatGptVs.Options
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("e8e21a31-43f2-4f39-aaf4-67d8709ea6ad")]
    public class ChatGptOptionsPage : DialogPage
    {
        private const string DefaultEndpoint = "https://api.openai.com/v1/chat/completions";

        [Category("Authentication")]
        [DisplayName("ChatGPT Login Token")]
        [Description("User token acquired via ChatGPT login flow. Stored locally on this machine.")]
        public string LoginToken { get; set; } = string.Empty;

        [Category("Service")]
        [DisplayName("Model")]
        [Description("ChatGPT model used for completions and edits. Defaults to gpt-4o")]
        public string Model { get; set; } = "gpt-4o";

        [Category("Service")]
        [DisplayName("Endpoint")]
        [Description("HTTP endpoint for the ChatGPT API.")]
        public string Endpoint { get; set; } = DefaultEndpoint;

        [Category("Suggestions")]
        [DisplayName("Max Suggestions")]
        [Description("Maximum number of completions returned per request.")]
        public int MaxSuggestions { get; set; } = 3;

        [Category("Suggestions")]
        [DisplayName("Replace Requires Confirmation")]
        [Description("When true, the extension will prompt the user before replacing existing code with a ChatGPT suggestion.")]
        public bool RequireConfirmationBeforeReplace { get; set; } = true;
    }
}
