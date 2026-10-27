# ADR 0001 — Native Windows product stack
Status: PROPOSED for approval, 27 October 2026.

Decision: C# WinUI 3 shell, supported .NET LTS, portable .NET core, Windows-specific adapters and an owned local model worker. Start with Windows 11 x64/CPU; add a measured NVIDIA profile. Keep ARM64 a distinct native-dependency lane.

Why: Microsoft recommends WinUI for new native desktop UI; the product needs Windows audio, HWND, UIA, keyboard and media integration. C# provides a maintainable application/domain layer without carrying the Mac Swift/AppKit/Metal assumptions into Windows. WinUI still requires explicit Win32/COM interop for some desktop behaviors.

Alternatives: WPF is mature for desktop interop and remains the fallback if the feasibility gate proves a material WinUI blocker. Avalonia can share a .NET UI across systems, but the user requested a separate Windows product and the Mac app is already native SwiftUI; it would not automatically share the current UI code. Electron/Tauri would not remove audio/input/inference integration requirements and add a different UI/runtime surface; they are not the default here.

Current research references: .NET 10 LTS and Windows App SDK stable 2.5.1, as-of-date catalog evidence. These are **candidate toolchain versions**, not an approved lockfile or a claim that the combination has built. The actual Windows template, XAML compilation, native package compatibility and launch must pass Phase 1.

Sources/corrections: F3.1–F3.4, F3.11–F3.12 in [the report](../../research/30-final-report.md). Revisit only with recorded spike evidence, not a speculative framework swap.
