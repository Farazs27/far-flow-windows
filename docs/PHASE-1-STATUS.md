# Phase 1 execution

Approved by the owner on **27 October 2026**. A native Windows evaluation preview is implemented and has passed the [Windows CI smoke run](https://github.com/Farazs27/far-flow-windows/actions/runs/40516886418). The original research and `planning/evidence-status.json` remain a preimplementation snapshot.

**[Download preview 0.1.0](https://github.com/Farazs27/far-flow-windows/releases/tag/v0.1.0-preview.1)** · [Tester checklist](TESTING-PREVIEW.md) · [Machine-readable evidence](../validation/0.1.0-preview/README.md)

Release `v0.1.0-preview.1` is public. The ZIP is built from `8f6ef2424745081ff0d54293b753d6e576244dae`; its SHA-256 is `c824e74501527250f3e50996b6003052747b9c77524d7627ad790b8fe51bb5e5`. [Current machine-readable status](../planning/current-execution.json).

## Implemented

The independent C#/WinUI shell uses the approved flowing F, travertine artwork, Zalando Sans and ivory/charcoal themes. It includes an explicit pinned/hash-verified 670 MB Parakeet v3 installation, a local CPU worker over current-user named pipes, creation-time kill-on-close Job Object ownership, WASAPI shared-mode microphone capture, hold/double-tap F8, a small draggable nonactivating waveform bar, session text/Copy and restricted Notepad delivery. Escape cancels; microphone cleanup is bounded and stale sessions cannot deliver new text. The preview ZIP includes .NET, Windows App SDK and the official Visual C++ CRT.

The official WinUI scaffold was instantiated and built on Windows. Direct-launch publication keeps canonical MSIX resource tooling enabled to generate the PRI/XAML resources, then stages the branded image/font assets explicitly. Tests confirm that the images actually render. Production signed MSIX remains a separate delivery gate.

| Evidence | Status and scope |
| --- | --- |
| Portable contract checks | **PASS**, 22 gesture/audio/IPC/integrity cases |
| Native x64 app / worker / probe build | **PASS**, Windows 2025 / Visual Studio 2026 runner, .NET 10.0.403 |
| Actual responsive window and brand images | **PASS**, light and dark app captures plus model setup navigation |
| Real app reaches Local & Ready | **PASS**, local CPU worker loaded through the actual app |
| Explicit model download and SHA-256 verification | **PASS**, every approved file downloaded/verified in CI |
| Real English/Dutch waveform recognition offline | **PASS**, two pinned public WAVs with worker outbound network access blocked; not a quality benchmark |
| Silence rejection | **PASS**, no text in the tested silence fixture |
| Normal app window close / model unload | **PASS**, loaded worker terminates |
| Forced parent termination | **PASS**, no surviving owned worker |
| Notepad Unicode and changed-focus recovery | **PASS**, runner Notepad; Dutch accents round-trip and changed target rejects delivery |
| Physical microphone, keyboard and cursor workflow | **Pending invited Windows tester** |
| Parakeet-versus-Whisper comparison / 200 reviewed voice clips | **Pending**, Parakeet is a preview candidate, not the final quality winner |
| Consumer Windows 11 / Bluetooth / low-memory / MSI/RTX testing | **NOT RUN** on real devices |
| Signed installer / production updates / seven-day pilot | **NOT RUN**, later release gates |

## What remains in Phase 1

The preview enables the friend's first Windows test; it does not close the entire Phase 1 stop/go gate. Review a real microphone → recognition → Notepad session on the intended Windows machine, gather personal English/Dutch/mixed samples, compare shortlisted CPU models, and finish the chosen package/media/permission feasibility spike. Only after those findings should broader editor support, GPU profiles and the full workspace proceed.

The preview intentionally uses CPU inference and direct Notepad insertion. Other apps use Copy. Media pause/resume, shortcut customization, persistent history, dictionary/snippets and writing tools are not advertised as implemented. Windows 11 x64 is the first product target; the hosted Server runner is build/smoke evidence, not consumer-hardware certification.

[Build instructions](BUILDING.md) · [Master plan](IMPLEMENTATION-PLAN.md) · [Release contract](VALIDATION-AND-RELEASE.md).
