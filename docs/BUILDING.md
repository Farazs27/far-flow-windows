# Build on Windows

The app is rooted in `Microsoft.WindowsAppSDK.WinUI.CSharp.Templates` 0.0.7-alpha, instantiated with `dotnet new winui` on Windows. That template version lacks the newer `--unpackaged` option; the preview deliberately sets `WindowsPackageType=None`, keeps the standard app/manifest startup structure and preserves MSIX resource tooling for XAML compilation/publication. No web view is used.

Use Windows 11 x64, Visual Studio 2026 with the WinUI/Windows App SDK C# and desktop components, Windows SDK 10.0.26100, and .NET SDK 10.0.403. GitHub CI uses the explicit `windows-2025-vs2026` image and records a real launch. The existing Mac cannot run the Windows executable.

From the repository root in PowerShell:

```powershell
./scripts/build-windows.ps1
./artifacts/FarFlow-Windows-x64/FarFlow.Windows.exe
```

Open `FarFlow.Windows.slnx` in Visual Studio to edit the projects. Use x64. The complete Phase 1 run loop uses the script above: it publishes both the UI and its `ModelHost` child with self-contained .NET runtimes and includes the official Visual C++ redistributable files from the installed VS toolchain. Building or F5-launching just the UI project does not stage its speech worker; use the combined script when testing dictation. Keep each published folder intact. The preview is a folder ZIP, not a single-file executable or signed installer.

`dotnet run --project tests/FarFlow.Checks/FarFlow.Checks.csproj` runs portable contract checks. The additional Windows probe and smoke scripts test pinned public English/Dutch WAVs with worker network access blocked, silence rejection and child shutdown. Test fixtures and model weights are downloaded for CI testing only, not committed or included in the application ZIP. Physical microphone and keyboard tests remain separate.
