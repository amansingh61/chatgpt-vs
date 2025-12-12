# ChatGPT Coding Companion for Visual Studio 2026

A Visual Studio 2026 VSIX extension that mirrors the GitHub Copilot experience while authenticating with ChatGPT. It delivers code completions, inline suggestions, and guarded replacements that require explicit user acceptance before applying edits. The design aligns with Microsoft extensibility guidelines for asynchronous loading, MEF-based editor components, and option pages.

## Features
- **ChatGPT login**: Provide a ChatGPT-issued token in the extension Options page. No GitHub account is required.
- **Auto-complete and suggestions**: An `IAsyncCompletionSource` feeds ChatGPT-backed suggestions into IntelliSense for any `code` content type.
- **User-approved replacements**: A Tools menu command fetches a suggestion for the current selection and prompts for approval before replacing code.
- **Configurable models**: Choose the ChatGPT model, endpoint, and maximum suggestions from the Options UI.

## Project layout
- `src/ChatGptVs/ChatGptVs.csproj` – VSIX project with VS SDK references.
- `src/ChatGptVs/source.extension.vsixmanifest` – Extension identity and assets.
- `src/ChatGptVs/ChatGptVsPackage.cs` – Async package registering options and commands.
- `src/ChatGptVs/Options/ChatGptOptionsPage.cs` – Options page capturing ChatGPT login token and tuning knobs.
- `src/ChatGptVs/Services/ChatGptClient.cs` – Minimal ChatGPT HTTP client for completions.
- `src/ChatGptVs/Services/SuggestionSessionManager.cs` – Mediates completion requests from editor components.
- `src/ChatGptVs/Completion/ChatGptCompletionSource.cs` – Async completion source delivering suggestions inline.
- `src/ChatGptVs/Completion/ChatGptCompletionSourceProvider.cs` – MEF provider wiring completion source into Visual Studio editor.
- `src/ChatGptVs/Commands/ApplySuggestionCommand.cs` – Tools menu command that previews and confirms replacements.
- `src/ChatGptVs/ChatGptVs.vsct` – Command table defining the Tools menu entry.

## Usage
1. Install the VSIX into Visual Studio 2026.
2. Open **Tools > Options > ChatGPT Coding Companion** and paste your ChatGPT login token.
3. Start typing in a code file to see inline suggestions; accept them through IntelliSense.
4. Select code and run **Tools > Apply ChatGPT Suggestion** to preview a replacement. Confirm to apply or cancel to keep existing code.

> **Note:** This repository provides a reference-quality implementation skeleton. The VSIX build requires the Visual Studio SDK and will need restoration of NuGet packages in a Visual Studio 2026 environment.
