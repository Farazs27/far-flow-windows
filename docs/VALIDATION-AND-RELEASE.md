# Windows acceptance and release contract

All application rows below start **NOT RUN**. Planning documentation checks are separate from Windows build, launch, model, UI and hardware evidence. This contract defines what must pass before the Windows edition is called fully working.

## Evidence ladder

| Evidence | What it establishes | What it does not establish |
| --- | --- | --- |
| Research and artifact listings | APIs/candidates/provenance worth testing | A functioning Far Flow binary |
| Portable core tests | Rules/schema/state-machine behavior in isolation | Real microphone/hook/insertion behavior |
| Windows native build/package | Toolchain resolves and compiles selected targets | Successful interactive launch or hardware operation |
| Native window/adapter smoke | A real responsive app and selected OS integration | All editors/devices/PCs supported |
| Offline model benchmark | Exact artifact/provider correctness and measured component performance | Full cursor delivery or universal speaker accuracy |
| Full pipeline matrix | Specific target/voice/device/configuration acceptance | Arbitrary protected/elevated software support |
| Fresh-machine install + pilot | Product use and distribution under tested conditions | A guarantee for every PC/driver |

## Reference machines

Record exact inventory before the first run. Obtain a CPU-only Intel lane, a CPU-only AMD lane and a 16 GB+ NVIDIA RTX 3070-class lane; a real minimum-8 GB lane must pass before advertising that floor. Prefer both a laptop and desktop/hybrid-GPU case. Do not substitute an Apple Silicon Windows ARM VM for x64/NVIDIA validation. A VM is useful for install/snapshot testing but is not proof of physical Fn keys, Bluetooth microphones, GPU inference or latency.

Target serviced Windows 11 x64 releases and editions. Include standard user, fresh profile and no developer tools. Add non-ASCII and spaced account/model paths (Dutch accents included), multiple displays, 100/125/150/175/200% scaling, supported NVIDIA driver and GPU-disabled operation. Native ARM64 is a future separate lane with matching native dependency proof.

## Required test groups

| ID | Scenario / test | Required result |
| --- | --- | --- |
| ENV-01 | Clean selected Windows host with pinned toolchain | Reproducible restore/build/package and real responsive main window |
| ENV-02 | CPU feature/runtime/DLL preflight | Supported ISA/ABI selected; clear unsupported result rather than illegal-instruction crash |
| AUD-01 | Actual endpoint formats, mono/stereo, PCM/Float32 | Correct 16 kHz mono ASR input and duration, finite samples, no format crash |
| AUD-02 | Bluetooth communications changes; 44.1 output / 48 input; 8–96 kHz cases | Native capture format negotiated per endpoint; converter rebuilt when needed |
| AUD-03 | Unplug/default-device switch/privacy denial/suspend/resume | Capture stops or recovers explicitly, no stuck mic or reused stale buffers |
| AUD-04 | 200 start/cancel/finish cycles; late callbacks | One active capture; new sessions cannot consume old buffers; no process exits/leaks |
| KEY-01 | Hold/release, double-tap, single short tap, repeat, boundary timestamps | Immediate first-press capture; correct release classification and lock/stop |
| KEY-02 | Lost release, slow UI, clock wrap, lock/sleep, other modifiers/layouts | No indefinite recording, duplicate insertion or AltGr corruption |
| KEY-03 | Actual chosen physical key on MSI/gaming/laptop/external keyboards | Setup sees real events; rebinding/conflicts work; no universal Fn claim |
| ASR-01 | Reviewed 80 EN / 80 NL / 40 mixed clips | Raw WER/critical tokens scored independently of cleanup; same corpus across candidates |
| ASR-02 | Silence/noise/very short words/empty buffers/max duration | No invented insertion in silence fixtures; defined short-word recovery and bounds |
| ASR-03 | GPU unavailable/missing DLL/driver change/low VRAM | Proven CPU fallback or clear recovery; no fake GPU-success label |
| ASR-04 | Exact int8/FP32/other GPU graph and preprocessing | Correct tokens/features/external weights; effective EP and node placement recorded |
| TXT-01 | Unicode boundaries, accents/composed forms, aliases, snippets | Noncascading longest rule; whole-trigger expansion; no accidental inside-word rewrite |
| TXT-02 | Numbers, dates, currency, email, URL, code identifiers, negations | Accepted cleanup preserves protected fixtures or returns original/review state |
| TXT-03 | Writer checkpoint/template and mixed/short language | No hidden thinking output, silent translation or unsupported semantic-safety claim |
| INS-01 | Cursor/selection captured at stop, then focus changes before/during delivery | No wrong-field insertion; no auto retry after ambiguous/partial output |
| INS-02 | Password/UAC/lock/elevated/unreadable provider | Product blocks or recovers to Copy; no blanket admin/UIAccess workaround |
| INS-03 | Partial SendInput, stale UIA element, hung COM provider | UI remains responsive, bounded recovery, honest result confidence |
| CLP-01 | User copies text while temporary paste is in flight | User's later clipboard wins; no overwrite during restore |
| CLP-02 | Text/HTML/image/delayed/private formats and cloud history | Supported formats preserved safely; original privacy flags respected; no Far Flow history/cloud entry introduced by temporary write or restore |
| TERM-01 | Multiline terminal text and trailing newline/control characters | Review/copy-only; never automatic execution; no added Enter |
| BAR-01 | Ready/recording/processing/cancel/failure; drag/hover | Waveform-only recording, correct mic indicator, no editor focus theft |
| BAR-02 | Active-window tracking hover, mixed DPI, monitor removal/taskbar moves | WM_MOUSEACTIVATE guard, stable scaled geometry, visible clamped position |
| MED-01 | Playing/paused player, player changes/disappears, user intervention, failed Try* | Only successful owned pause is eligible for resume; never starts a new player |
| LIFE-01 | Main-window X/Quit during each state | Microphone/hook/overlay/tray stop; owned workers gone after bounded shutdown |
| LIFE-02 | Force-kill parent during worker creation and active inference | No orphan model worker; creation-time job assignment and no leaked/inherited owner handle |
| LIFE-03 | Minimize/open at login/update interactions | Minimize stays available; login opt-in; updates do not interrupt/duplicate sessions |
| DATA-01 | Full backup and dictionary-only JSON, corrupt/future schema, disk full | Explicit schema path, reviewed import, pre-import backup, no silent data reset |
| DATA-02 | Retention Off/day/month/keep and manual clear | Both raw/cleaned retention enforced; notes/dictionary not accidentally deleted |
| MOD-01 | Interrupted/partial/malicious-path model downloads/imports | Size/hash/path/format validation before activation; old working model retained |
| NET-01 | Network denied after explicit install, including DNS/loopback observation | Full advertised local workflow works; no auto model/tokenizer fetch or inference server |
| UI-01 | Light/dark/High Contrast, text scaling, keyboard/Narrator | Clear accessible controls/focus/status, usable adaptive layout |
| PKG-01 | Fresh install/update/rollback/uninstall on standard-user PC | Expected publisher/signature/icons, no dev-tool dependency, documented data/model outcome |
| PERF-01 | Cold/warm CPU/NVIDIA/battery/thermal/low-memory runs | Proposed targets measured with exact configuration; no RSS-only GPU-memory claim |
| PILOT-01 | Seven-day real English/Dutch replacement use | Reviewed defects, no open release blockers, user acceptance of actual compatibility |

## Initial application insertion matrix

Exercise middle-of-text caret insertion, selected replacement, emoji/accented text, mixed language, focus switch and copy recovery in each target. Record actual app/version/OS and delivery mode/outcome. Do not label “supports Windows” based on one Notepad paste.

| App family | First reference targets | Status |
| --- | --- | --- |
| Native simple editor | Notepad, Windows native text boxes | NOT RUN |
| Office | Word desktop, Outlook desktop | NOT RUN |
| Browser editors | Edge and Chrome textareas/contenteditable; Gmail/Docs-style editors | NOT RUN |
| Electron | VS Code editor, Slack/Discord/Teams where available | NOT RUN |
| Terminal | Windows Terminal PowerShell/cmd, VS Code terminal | NOT RUN — multiline copy-only policy |
| Elevated/protected | Administrator Notepad, password fields, UAC/lock screen | NOT RUN — expected block/recovery, not insertion support |
| Unsupported contexts | Anti-cheat/protected games, arbitrary remote sessions/providers | Excluded from v1 guarantee; document actual behavior |

## Performance and accuracy accounting

Use [model gates](COMPATIBILITY-AND-MODELS.md) as initial targets. Measure start trigger→first actual buffer, stop event→verified text at intended caret, model load separately from warm compute, p50/p95, failures, working set of all owned processes, and dedicated/shared GPU allocations. A delivery request without verified readback is a different outcome, not a verified insertion latency sample.

Report raw ASR quality, cleanup invariants and insertion reliability separately. A correct rewrite cannot hide a wrong raw number; a correct transcript cannot hide wrong-field delivery. Synthetic speech and M4 timings are not evidence for Windows personal-voice quality.

## CI versus real-device testing

Planning CI currently validates this repository's documents/assets/metadata only. During implementation, add Windows restore/build, portable core and adapter-contract tests, package artifact checks and selected offline inference smoke tests. Pin the chosen runner/toolchain rather than `windows-latest` assumptions. Research found `windows-2025` carrying VS2022 and a separate `windows-2025-vs2026` image; explicitly provision the SDK selected for the project and record the actual image inventory. [F3.12 corrected]

Audio endpoints, Bluetooth, a real active desktop, external-editor caret behavior, physical keyboard events, NVIDIA performance and installer trust/reputation require interactive/reference-device runs. A green GitHub build does not replace them.

## Release blocker policy

Any wrong-target insertion, protected-field delivery, automatic multiline terminal execution, lost/newer clipboard overwrite, unintended media resume, silent cloud fallback, active microphone after cancel, orphan worker after close/crash, or data-loss migration is a release blocker. The same applies to failing baseline model quality, package install, or a misleading verified-success outcome. Cosmetic issues may be triaged, but not used to redefine a failed invariant as acceptable after the fact.

Release artifacts must include exact version, supported OS/architectures, known limits, signed package/checksums, SBOM/notices, model provenance/manifest, genuine Windows images and dated test results. Code-signing/reputation and certificate trust are not simulated by a developer build. No release is created by this planning stage.
