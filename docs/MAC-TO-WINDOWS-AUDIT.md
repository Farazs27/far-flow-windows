# Mac-to-Windows migration audit

Audited public Mac baseline: [0.2.1, ccb628ad](https://github.com/Farazs27/far-flow/tree/ccb628adbeea97ed898c35f1f83b21188815c32b). Scope and independent corrections: [dossier 4](../research/10-findings-4.md), [validation 4](../research/20-validation-4.md). This is a behavioral port; there is no shared Swift binary assumption.

| Mac component / feature | Windows treatment | Validation requirement |
| --- | --- | --- |
| SwiftUI/AppKit workspace, RootView/navigation | Rebuild in WinUI 3; same content/brand, native caption/focus/menu behavior | Real Windows resize/theme/keyboard/Narrator evidence |
| FlowMark / studio PNG / travertine banner | Reuse approved source geometry/artwork; create Windows ICO/package tiles during implementation | Actual Explorer, taskbar, Alt+Tab and installed-app icons across update |
| Zalando Sans | Bundle original font + OFL notice; system fallback and High Contrast | Scaling, glyphs, ligatures/accents, accessible fallback |
| SF Symbols / Apple Tink and Pop | Replace with independently licensed Windows glyphs and original subtle earcons | Do not extract OS/competitor assets; app-only gain/toggle |
| AVAudioEngine / Core Audio / Obj-C exception guard | Replace with WASAPI/Windows adapter; preserve ownership/recovery contracts | Endpoint sample format, Bluetooth mode, repeated capture and device loss |
| FnGesture finite-state behavior | Re-specify/port logic and edge tests; first press starts capture immediately | Boundaries, repeats, lost release, scheduling delays, lock/sleep |
| CGEvent Accessibility event tap | Dedicated Windows key-edge path; configurable ordinary trigger | Rebinding/conflicts, layouts/AltGr, hook responsiveness, physical device checks |
| Fn default | F8 proposed configurable Windows default; Fn only on detected supported hardware | Actual event-detection setup and user-selected binding |
| Core ML / FluidAudio speech | New Windows inference backend + new converted files/manifest | CPU English/Dutch/mixed model benchmark, exact preprocessing/provider |
| MLX writer artifacts/bindings | New GGUF/runtime adapter with exact checkpoint provenance | Non-thinking writer identity, template, output limits, safety and memory |
| NaturalLanguage NLLanguageRecognizer | New small local language-ID strategy; conservative mixed/unknown behavior | Short/mixed-language corpus; explicit EN/NL hints work without false translation |
| Dictionary / snippets / Light rules | Translate specifications and create platform-neutral golden fixtures | Unicode/culture boundaries, noncascading replacements, whole triggers, punctuation |
| Protected-span/negation/language checks | Port rules and expand fixtures; do not call them a semantic proof | Numbers, currencies, URLs, email, identifiers, dictionary names and EN/NL negation |
| InsertionService AX target/caret | Windows target snapshot at stop; UIA inspection + tested delivery adapters | No whole-document overwrite; protected/elevated/changed targets recover safely |
| Clipboard restore | Windows ownership/sequence-aware implementation + history/cloud exclusion | New user copies win; unsupported formats do not get silently destroyed |
| Terminal handling | Detect supported terminal targets; multiline review/copy-only, no appended Enter | PowerShell/cmd/Windows Terminal/VS Code terminals and multiline safety |
| RecordingPanel NSPanel | Separate nonactivating HWND; same small waveform-only concept | Hover/drag never steals focus; DPI/work-area/monitor recovery |
| MediaRemote Perl/Obj-C helper | Replace entirely with public GSMTC session adapter | Explicit pause ownership, session changes/user intervention/failure matrix |
| SQLite LocalStore | New .NET persistence adapter; preserve transactions/schema safety | Corrupt/future schema, disk full, retention, interrupted import and backup |
| Full JSON backup | Reviewed schema-1 ISO-8601 adapter | Do not confuse dictionary-only JSON or raw SQLite serialization with full backups |
| AppStyle bundleID | New Windows app identity/matching adapter; review imported targets | Executable/AUMID/path rules, same-name apps, no blind Mac identifier reuse |
| Core Audio UInt32 microphone IDs | Reset to System default or user-selected Windows endpoint | Stable endpoint identity and unplug/replug behavior |
| Transforms / scratchpad / insights | Rebuild UI around portable product behavior | Review before replacement; exact verified undo only; retained-data insights |
| Model manager/checksums | Port contracts; Windows path/ACL/Unicode/native-DLL loading specifics | Staging, hash validation, path traversal, incomplete download/import, rollback |
| Close/minimize/Open at login | Explicit Windows owner lifetime/tray/startup behavior | Close quits workers/mic; minimize preserves availability; login is opt-in |
| Xcode/codesign/directory-copy installer | New Windows build/package/signing/update pipeline | Fresh standard-user install, update, rollback, uninstall/data choice |
| Mac tests and synthetic timings | Reuse test *ideas* and invented data; run Windows equivalents | Mac test passes and M4 component speeds never relabeled as Windows results |

## Additional porting details

C#/.NET and Foundation differ in culture-sensitive case comparison, Unicode normalization, grapheme versus UTF-16 length and regular-expression behavior. Preserve visible meaning and explicit invariants, not accidental string-library behavior. A default language selection can bypass detection, but does not solve mixed/Auto language labeling.

Current full backup export/import explicitly uses ISO 8601 dates. Internal SQLite-body JSON uses Foundation defaults; copying the live database would introduce a different migration problem, including WAL/transaction consistency. The Windows importer should recognize a full backup envelope, validate schema and size, preview affected collections, preserve a pre-import backup and reset platform-specific settings. Dictionary-only JSON gets its own import path.

Keep user data out of this repository. Test fixtures are invented examples; never turn a user's retained history or speech into public acceptance data.

The Mac verification record reports 50 passing tests and M4 synthetic measurements. Cross-validation corroborated 50 test definitions, but did not independently rerun XCTest. This planning session has no Windows build/run results. [F4.8 corrected]
