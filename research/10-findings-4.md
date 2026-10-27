# Dossier 4: Actual Mac implementation and migration boundaries
Researcher scope: current source audit and compatibility requirements | Goal: Windows architecture/approval plan | Date: 27 October 2026

## Search log
- Read the current Mac `project.yml`, application services and core implementations at public commit `ccb628adbeea97ed898c35f1f83b21188815c32b`.
- Followed persistence export/import paths rather than assuming that a SQLite JSON blob and an exported backup use identical date encodings.
- Read dictation/insertion/termination code and the 50-test verification record.
- Disconfirming search: looked for hidden portable-language assumptions in the supposedly reusable text pipeline; it imports Apple NaturalLanguage and uses NLLanguageRecognizer, so it is not directly portable as-is.

## Findings

### F4.1 — The app shell is a native Mac implementation
- **Claim:** The current project targets macOS 26/arm64 and its shell/services use SwiftUI, AppKit and ApplicationServices. No Windows application target is present in this project definition.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/project.yml — primary project configuration — inspected 27 October 2026; https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlow/App/InsertionService.swift — primary source — same date.
- **Confidence:** High.
- **Notes:** Reusable product behavior does not imply reusable native binaries or a compilable Windows shell.

### F4.2 — The inference artifacts and bindings are Mac-specific
- **Claim:** The current speech path uses FluidAudio Core ML assets and the writing path uses MLXLLM/MLXLMCommon. The model manifest describes a Core ML Parakeet conversion and an MLX Qwen conversion, not Windows ONNX/GGUF release artifacts.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlowCore/Inference.swift — primary implementation — inspected 27 October 2026; https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlow/Resources/ModelManifest.json — primary manifest — same date.
- **Confidence:** High.
- **Notes:** Source-model identity and original attribution can carry over; runtime binaries, converted artifacts, checksums and provider validation must be selected afresh.

### F4.3 — Gesture behavior can be re-specified independently of Fn
- **Claim:** The current FnGesture state machine uses a 0.22-second hold threshold and 0.35-second double-tap window, with start/stop/lock outputs. The Windows plan can port this behavior as specifications/tests while replacing the key event source.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlowCore/FnGesture.swift — primary algorithm — inspected 27 October 2026.
- **Confidence:** High for existing behavior; proposed portability is an engineering inference.
- **Notes:** These are current defaults, not a human-factors benchmark or a claim that Fn is exposed on Windows keyboards.

### F4.4 — JSON exports are the intentional interoperability boundary
- **Claim:** LocalStore export and import explicitly use ISO 8601 dates with schemaVersion 1. Internal SQLite body serialization instead uses JSONEncoder/JSONDecoder defaults. An exported JSON file is therefore the intended portable import boundary, rather than copying a live database or assuming its date representation.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlowCore/LocalStore.swift — primary persistence source — inspected 27 October 2026.
- **Confidence:** High for code behavior; Windows import remains unimplemented.
- **Notes:** Windows needs an explicitly versioned schema adapter; UUID/date/enum/optional fields and unknown schema handling need fixtures. Never upload real user exports as fixtures.

### F4.5 — Text behavior has portable rules plus an Apple language dependency
- **Claim:** Dictionary replacements are noncascading and use Unicode letter/number/underscore boundaries; snippets compare the whole normalized trigger. Language detection uses NLLanguageRecognizer, so reproducing the text pipeline requires replacing that dependency and validating Unicode/culture behavior.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlowCore/TextPipeline.swift — primary source — inspected 27 October 2026.
- **Confidence:** High.
- **Notes:** Do not silently translate mixed speech or assume .NET Regex and culture comparison exactly match Foundation/ICU defaults. Use an explicit English/Dutch choice and tested language-ID behavior where automatic detection is uncertain.

### F4.6 — Existing app-style identifiers need a Windows adapter
- **Claim:** The Mac AppStyle model stores bundleID, while preferences include an integer Core Audio device ID and the default shortcut string fn. These values cannot be treated as validated Windows executable identities, stable Windows microphone endpoint IDs or a universally available shortcut.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlowCore/Models.swift — primary data model — inspected 27 October 2026.
- **Confidence:** High for schema; Windows mappings are proposed design decisions.
- **Notes:** Import dictionary/snippets/notes/history deliberately; reset/remap platform preferences and app-style targets with user review rather than guessing.

### F4.7 — Main-window close deliberately quits the Mac app
- **Claim:** The current delegate/close observer requests application termination; termination cancels capture and tasks, closes the panel, stops the media helper and unloads models. Minimize remains distinct from close. This behavior is part of the current product contract to preserve on Windows.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlow/App/FarFlowApp.swift — primary lifecycle code — inspected 27 October 2026; https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlow/Views/RootView.swift — primary close-observer source — same date.
- **Confidence:** High.
- **Notes:** Windows workers and tray icons need explicit ownership; no claim that the Mac mechanism can be copied into WinUI.

### F4.8 — Existing test and timing evidence is not Windows evidence
- **Claim:** The current verification record reports 50 passing Mac unit tests and synthetic component benchmarks on an M4 Mac. Those results do not establish Windows launch, CPU/GPU latency, dictation accuracy or cross-app compatibility.
- **Source:** https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/docs/VERIFICATION.md — primary project evidence — updated 27 October 2026.
- **Confidence:** High.
- **Notes:** Personal-voice and Windows hardware benchmarks remain required. The existing synthetic corpus is useful as a regression starter, not an acceptance substitute.

### F4.9 — Brand source assets and font notices can be kept separate from Mac icon packaging
- **Claim:** The repository contains a scalable flowing-F SVG, PNG icon master, travertine banner and Zalando Sans font with its SIL OFL notice. Mac appiconset/ICNS packaging and SF Symbols usage are separate platform-specific implementation details.
- **Source:** https://github.com/Farazs27/far-flow/tree/ccb628adbeea97ed898c35f1f83b21188815c32b/brand/assets — primary artwork sources — inspected 27 October 2026; https://github.com/Farazs27/far-flow/blob/ccb628adbeea97ed898c35f1f83b21188815c32b/FarFlow/Resources/Fonts/ZalandoSans-OFL.txt — primary bundled font license — same date.
- **Confidence:** High for available assets/notices.
- **Notes:** The user has authorized reusing their Far Flow branding. Keep the root rights reservation and third-party font terms distinct. Do not extract proprietary competitor artwork or redistribute SF Symbols as a Windows icon pack.

## Gaps — what I could not find
- No Windows application project, Windows installer, Windows model/provider benchmark or Windows live input compatibility evidence exists in the audited Mac tree.
- No exact Windows test PC, RAM or GPU configuration has been supplied. MSI/3070 is a requested hardware class.
- No portable cross-platform language-ID implementation or Windows application-style mapping has been implemented.

## Disconfirming evidence check
- The text pipeline was initially a plausible direct-translation candidate, but Apple NaturalLanguage makes language detection a real dependency migration.
- A raw SQLite-body copy would wrongly treat internal date handling as equivalent to the explicit ISO 8601 export format.
- Current Mac feature tests and benchmark results cannot be relabeled Windows results.
