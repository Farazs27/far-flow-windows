# Phase 1 execution

Approved by the owner on 27 October 2026. The original research and `planning/evidence-status.json` are a preimplementation snapshot; this document records subsequent execution.

The official Microsoft WinUI template was instantiated and built on a Windows 2025 / Visual Studio 2026 runner with .NET SDK 10.0.403. The initial unpackaged publish omitted the required compiled resources when MSIX tooling was disabled; the canonical resource tooling is being retained while testing direct-launch publication.

Implemented for the first preview: branded native shell, explicit pinned/hash-verified Parakeet v3 download, local CPU worker over current-user named pipes, creation-time Job Object ownership, shared-mode microphone capture, hold/double-tap F8, waveform-only nonactivating bar, session text/Copy and restricted Notepad insertion. Production signing, general editor delivery, NVIDIA acceleration and the complete workspace are outside this preview.

| Evidence | Current status |
| --- | --- |
| Portable core build | PASS on macOS; no Windows UI claim |
| 22 gesture/audio/IPC/integrity contract checks | PASS locally |
| Official template Windows build | PASS |
| Native app Windows build and responsive launch | In progress |
| English/Dutch real-wave offline smoke | In progress |
| Worker graceful/forced lifetime test | In progress |
| Physical microphone, keyboard and cursor tests | Pending invited Windows tester |
| CPU model comparison / 200 reviewed natural-voice clips | Pending; Parakeet remains a candidate |
| NVIDIA/MSI/RTX 3070 certification | NOT RUN |
| Signed MSIX / production update channel / seven-day pilot | NOT RUN |

The Windows CI workflow preserves actual window and ASR evidence. A successful smoke is a narrow proof, not a claim that all microphones, dialects, PCs or editors are certified. The Phase 1 stop/go gate stays open until the interactive end-to-end tester path and model-selection measurements are reviewed.
