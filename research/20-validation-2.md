# Adversarial validation 2: Windows OS integration

Validator: independent inference researcher cross-checking dossier 2 | Date: 2026-10-27 | Input: `10-findings-2.md`

Verdicts cover both the atomic API claim and material design implications in its notes. Nine findings are CONFIRMED; three are PARTIAL. No Windows application, keyboard, microphone, target editor, media player or worker was run. Confirmation means documentary support, not a Windows acceptance pass. Older stable API references were checked against different contract entry points, source code or maintained implementation documentation; a publication date by itself is not treated as proof of current runtime behavior.

## Independent search log

- `Fn key Windows firmware not recognized Lenovo site:support.lenovo.com` plus the Microsoft virtual-key inventory → OEM model-dependent Fn configuration and no documented `VK_FN` entry; no universal Fn contract found.
- `site.learn.microsoft.com WM_HOTKEY posted hot key pressed message` → separate WM_HOTKEY message contract; release messages have a different routing contract.
- `site.microsoft.com low level keyboard hook silently removed timeout LowLevelHooksTimeout` and `site.autohotkey.com/docs LowLevelHooksTimeout silently removed research 11th` → independent implementation-author observation, but no separate modern Microsoft contract for the exact one-second cap. Rejected circular confirmation from PowerToys issue 50980, which cites the researcher's API reference.
- `site.github.com/microsoft/Windows-classic-samples WASAPICapture GetMixFormat OnDefaultDeviceChanged` → working source structure for shared capture and explicit endpoint/format-change handlers.
- `site.learn.microsoft.com IValueProvider SetValue entire value text control` → provider-side interface confirms whole-value method shape; its legacy multiline-control prose is not a universal control-pattern contract.
- `site.learn.microsoft.com UIPI SendInput` and narrower Power Automate support search → separate current product troubleshooting evidence for same/lower-elevation constraints.
- `site.learn.microsoft.com About Keyboard Input foreground` → separate keyboard routing model.
- `site.learn.microsoft.com clipboard CanUploadToCloudClipboard CanIncludeInClipboardHistory` → overlooked clipboard history/cloud-sync controls.
- `WS_EX_NOACTIVATE tracking Raymond` → official counterexample: hover activation with active window tracking.
- `site.learn.microsoft.com GlobalSystemMediaTransportControlsSessionManager SystemMediaTransportControls`, manifest schema, and Microsoft-generated media bindings → cooperating-session scope, packaged capability syntax, Boolean async pause result.
- `site.learn.microsoft.com Job Objects CreateProcess PROC_THREAD_ATTRIBUTE_JOB_LIST` followed by `site.devblogs.microsoft.com/oldnewthing PROC_THREAD_ATTRIBUTE_JOB_LIST` → explicit official warning about the suspended-create/assign orphan window and creation-time job assignment.
- `site.learn.microsoft.com UI Automation Threading Issues` and `IUIAutomation2 ConnectionTimeout` → target validation can block/hang and needs a bounded separate execution path.

## Verdicts

### F2.1 — Fn is not a portable Windows dictation trigger

- **Verdict:** CONFIRMED, limited to the absence of a universal standalone-Fn promise.
- **Independent source:** [Lenovo Fn/Hotkey Mode guidance](https://support.lenovo.com/ca/en/solutions/ht103044), OEM primary support documentation, originally published 2016-12-21 and modified 2026-03-25; [Microsoft virtual-key inventory](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes), separate primary API inventory, repository metadata 2025-10-06, accessed 2026-10-27.
- **Correction / contradiction:** None to the atomic claim. Lenovo explicitly varies the method by machine family/model, including BIOS, Fn Lock and Lenovo software settings. The virtual-key inventory has no standalone `VK_FN` contract. These support a hardware-dependent binding, not a specific theory that every keyboard consumes Fn in firmware. No MSI Fn event evidence exists.
- **Source-quality flags:** The researcher's PowerToys source is primary documentation and directly supports its stated scope. Its Q&A mechanism explanation remains forum-grade, as already disclosed. OEM advice is model-specific and has a hardware vendor incentive, but does not market universal behavior; the dated guidance is current enough for that narrow scope.

### F2.2 — RegisterHotKey alone does not expose the release edge

- **Verdict:** CONFIRMED.
- **Independent source:** [WM_HOTKEY message](https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey), primary Microsoft API contract, updated 2025-07-14; [WM_KEYUP message](https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-keyup), separate primary contract, repository date 2025-07-14.
- **Correction / contradiction:** None. WM_HOTKEY documents a press notification and contains modifier/key identifiers, without a matching release flag. WM_KEYUP is directed to the focused window; ordinary key-up handling is not automatically global because a hotkey was registered. An edge-aware input implementation is necessary for the proposed hold semantics, but the evidence does not select hooks over Raw Input.
- **Source-quality flags:** None on the core contract. These are stable Win32 interfaces, not performance promises. The sources cross-link related functions, but the separate message contracts independently establish the distinction rather than repeating the RegisterHotKey prose.

### F2.3 — Slow keyboard hooks may be silently removed

- **Verdict:** PARTIAL: core removal risk independently supported; the exact modern timeout detail was not independently confirmed.
- **Independent source:** [AutoHotkey implementation-author timeout documentation](https://github.com/AutoHotkey/AutoHotkeyDocs/blob/7951621f191be3389005e95a707288c8c2da164a/docs/lib/_HotIfTimeout.htm), primary project documentation, file commit 2026-08-28. Its authors report their own research into silent hook removal and warn that callbacks can cease until reregistration.
- **Correction / contradiction:** Keep “a slow hook can stop receiving callbacks, so return immediately and hand work off.” Retain the one-second Windows 10 1709+ cap as a fact sourced to the researcher's Microsoft API reference, but do not label it independently cross-confirmed here. AutoHotkey discusses legacy observed defaults/repeated timeouts, which must not be substituted for a modern Windows 11 threshold. No precise physical-actuation or scheduling guarantee follows from either source.
- **Source-quality flags:** The AutoHotkey site returned 403, so the official project repository was read. It is first-party software-author guidance with an independent observation, but not an OS specification; historical experiments may be stale. PowerToys [issue 50980](https://github.com/microsoft/PowerToys/issues/50980), opened 2026-10-03, was rejected as independent confirmation because it quotes/links the same Microsoft reference. Its unmerged mitigation PR is not a Windows guarantee.

### F2.4 — WASAPI shared capture accepts its mix format

- **Verdict:** CONFIRMED.
- **Independent source:** [IAudioClient::IsFormatSupported](https://learn.microsoft.com/en-us/windows/win32/api/audioclient/nf-audioclient-iaudioclient-isformatsupported), separate primary Microsoft API contract, updated 2025-09-05; [shared-event capture sample source](https://github.com/microsoft/Windows-classic-samples/blob/8f31b1ff79d669b4ba9609f2640635b3b8a9e0a4/Samples/Win7Samples/multimedia/audio/CaptureSharedEventDriven/WASAPICapture.cpp), primary Microsoft SDK sample, last file commit 2016-05-19.
- **Correction / contradiction:** None to format support. IsFormatSupported explicitly states that shared mode always supports the mix format, and the sample passes the queried format to shared initialization. This means supported format, not guaranteed success in the presence of endpoint invalidation, resource failure, privacy denial or invalid flags. Format acceptance does not establish ASR-compatible rate, channels or sample representation.
- **Source-quality flags:** The sample is old and supplied as-is; the current live API contract provides the authoritative format guarantee. No latency, resampler-quality or all-device result is corroborated by the sample.

### F2.5 — Direct capture owns endpoint-change handling

- **Verdict:** CONFIRMED.
- **Independent source:** [Microsoft shared capture implementation](https://github.com/microsoft/Windows-classic-samples/blob/8f31b1ff79d669b4ba9609f2640635b3b8a9e0a4/Samples/Win7Samples/multimedia/audio/CaptureSharedEventDriven/WASAPICapture.cpp), primary SDK source, file commit 2016-05-19. It registers both endpoint and audio-session notifications and implements `OnDefaultDeviceChanged`, `OnSessionDisconnected` and explicit stream reconstruction.
- **Correction / contradiction:** No correction to responsibility. A material limitation of this particular sample is that it aborts if the new format differs: its own comments say a real application would rebuild its format pipeline. Copying this sample would not produce the dossier's desired seamless format recovery. That remains proposed work. The Bluetooth sample-rate numbers were not independently hardware-verified; they remain the researcher's device/build-specific official documentation, not a universal Bluetooth capability claim.
- **Source-quality flags:** Primary executable source independently corroborates responsibility; age and incomplete recovery limit what can be inferred about modern drivers. No new Windows test evidence.

### F2.6 — ValuePattern does not supply insertion at a caret

- **Verdict:** CONFIRMED.
- **Independent source:** [Provider-side IValueProvider::SetValue](https://learn.microsoft.com/en-us/windows/win32/api/uiautomationcore/nf-uiautomationcore-ivalueprovider-setvalue), separate primary Microsoft interface contract, updated 2024-02-22; [managed provider example](https://learn.microsoft.com/en-us/dotnet/api/system.windows.automation.provider.ivalueprovider.setvalue?view=windowsdesktop-10.0), primary sample implementation, undated living reference accessed 2026-10-27.
- **Correction / contradiction:** None to the method shape: the provider receives one new value with no insertion range. The managed example assigns the replacement control value. The provider page also contains legacy generalizations about multiline controls; that prose must not override the more specific current native Text/TextRange coexisting-pattern guidance already identified by the researcher. Never convert “can set a value” into “safe generic insert at selection.”
- **Source-quality flags:** Stable provider-side ABI and sample independently support replacement semantics, while pattern implementation varies across controls. Dates are older than six months, but no version-specific behavior or app coverage is inferred.

### F2.7 — Ordinary input injection cannot cross upward through UIPI

- **Verdict:** CONFIRMED.
- **Independent source:** [Power Automate UIPI troubleshooting](https://learn.microsoft.com/en-us/troubleshoot/power-platform/power-automate/desktop-flows/ui-automation/uipi-issues), separate primary Microsoft support article, updated 2026-04-08. It documents failed simulated input at higher elevation and additional failure contexts including locked desktops/UAC.
- **Correction / contradiction:** None to the upper-integrity prohibition. “Equal or lower” is a necessary boundary, not an assurance of successful injection into all applications at that level. Secure desktop, minimized/disconnected remote contexts, target input processing and access policy are separate limits. The article's troubleshooting workarounds are not authorization to weaken UAC or change the user's OS settings; the architecture should retain safe failure/copy recovery as proposed.
- **Source-quality flags:** Microsoft product support is primary and practical, with product incentive but no promotional benchmark. It corroborates the boundary independently of the researcher's SendInput reference. No elevated Windows target was exercised.

### F2.8 — Injection routing follows current foreground focus

- **Verdict:** CONFIRMED for routing; clipboard privacy remains an important omission below.
- **Independent source:** [Keyboard Input Overview](https://learn.microsoft.com/en-us/windows/win32/inputdev/about-keyboard-input), separate primary Microsoft routing contract, updated 2025-07-14. It describes foreground-thread/focused-window input delivery and the changeable focus lifetime.
- **Correction / contradiction:** None to the lack of a target-HWND parameter in this injection path. The documented model supports revalidation and abort on focus drift, but not an atomic cross-application commit or a consumed-text acknowledgment. “All requested INPUT events were queued” must not be recorded as “the exact final string was accepted.” The original terminal-newline restriction remains necessary; validation found no reason to weaken it. Clipboard restoration must not be called lossless merely because its sequence check succeeds.
- **Source-quality flags:** Stable primary routing documentation supports the narrow claim. It is not a per-control Unicode/IME/terminal compatibility result. Its general routing contract is separate from the researcher's KEYBDINPUT definition; no Windows runtime acknowledgment was fabricated.

### F2.9 — The nonactivating overlay requires another activation guard

- **Verdict:** PARTIAL.
- **Independent source:** [Raymond Chen's no-activate counterexample](https://devblogs.microsoft.com/oldnewthing/20240919-00/?p=110283), primary Microsoft engineering explanation, published 2024-09-19; [WM_MOUSEACTIVATE](https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-mouseactivate), separate primary API contract, undated live reference accessed 2026-10-27.
- **Correction / contradiction:** The ordinary-click statement is correct. However, the proposed overlay must also handle `WM_MOUSEACTIVATE` and return `MA_NOACTIVATE` or the deliberately chosen nonactivating/eating result. Microsoft's blog explicitly documents active-window-tracking hover activation bypassing the `WS_EX_NOACTIVATE` style. Add a hover-activation test with the accessibility setting enabled; styling and nonactivating show/move calls alone are incomplete. Pointer/touch behavior and native dragging still require Windows validation.
- **Source-quality flags:** The official engineering post is older than six months but describes a concrete window-manager exception that remains relevant until tested or superseded. It corroborates the main click behavior while invalidating a broader “style alone guarantees no focus theft” inference. The researcher's existing warning about explicit activation calls did not cover this user-setting path.

### F2.10 — GSMTC exposes participating sessions

- **Verdict:** CONFIRMED for the public participating-session boundary.
- **Independent source:** [Windows.Media.Control namespace](https://learn.microsoft.com/en-us/uwp/api/windows.media.control?view=winrt-26100), separate primary Microsoft namespace contract, undated live reference accessed 2026-10-27; [uap7:Capability manifest schema](https://learn.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-uap7-capability), primary schema reference, undated living page accessed 2026-10-27.
- **Correction / contradiction:** None. The namespace explicitly scopes the manager to apps integrated with SystemMediaTransportControls, and the schema includes `globalMediaControl` with a Windows 10 1809 minimum. Neither source proves control of arbitrary audio sessions. They also do not settle every packaged Medium-IL/unpackaged permission case, so the researcher's unresolved deployment matrix must remain unresolved. No mandatory consent prompt, MSIX exclusivity or universal playback-control claim should be added during synthesis.
- **Source-quality flags:** Primary contract and manifest schema; no marketing. API-view prerelease banners are broad documentation boilerplate, not evidence that this established API is newly experimental. The exact product release configuration still needs a standard-user Windows test.

### F2.11 — Pause has a Boolean result without causal ownership

- **Verdict:** CONFIRMED for the Boolean request result; ownership remains a policy inference.
- **Independent source:** [Microsoft-generated Windows media bindings](https://github.com/microsoft/windows-rs/blob/a6b6ef58f8efb2f2bdef86a2f6cd5f768eb7c545/crates/libs/windows/src/Windows/Media/Control/mod.rs), primary SDK projection source, file commit 2026-09-02. `TryPauseAsync` returns an async Boolean inside a fallible call, with separate playback state/event methods.
- **Correction / contradiction:** No correction to the return type. Source-level inspection shows no pause-ownership token or command-origin field in the exposed session surface. The independent projection verifies the ABI, while the researcher's separate method documentation supplies the true/success semantics; that semantic detail is not a new independent empirical test. Successful request status should not be elevated to causal proof that Far Flow alone caused the pause. Conservative invalidation under ambiguity is supported; reliable automatic ownership in every race is not established.
- **Source-quality flags:** Current Microsoft-authored generated source is primary but is an API projection, not the internal media service implementation. Absence from a public surface supports “no documented exposed token,” not a claim about all private internals or all third-party players.

### F2.12 — Kill-on-close works, but the suggested creation sequence has an orphan window

- **Verdict:** PARTIAL.
- **Independent source:** [CreateJobObjectW](https://learn.microsoft.com/en-us/windows/win32/api/jobapi2/nf-jobapi2-createjobobjectw), separate primary API contract, updated 2022-07-27; [Microsoft engineering explanation of creation-time job assignment](https://devblogs.microsoft.com/oldnewthing/20230209-00/?p=107812), published 2023-02-09; [UpdateProcThreadAttribute](https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-updateprocthreadattribute), separate primary contract, updated 2022-11-01.
- **Correction / contradiction:** The kill-on-last-handle-close claim is confirmed. The suggested create-suspended → assign → resume sequence is incomplete for a no-orphan objective: the owner can die after creation and before assignment, leaving a suspended orphan. Microsoft's article identifies that exact failure. For Windows 10+ creation, use `PROC_THREAD_ATTRIBUTE_JOB_LIST` through `STARTUPINFOEX`/`EXTENDED_STARTUPINFO_PRESENT` so assignment occurs before the initial thread runs, with kill-on-close set beforehand and error handling for incompatible nested jobs. Use an explicit inherited-handle allowlist for IPC and exclude the job handle. Retain the UI as sole intended job-handle owner.
- **Source-quality flags:** Primary contract and independently written engineering explanation, not a recycled copy of the researcher's Job Objects page. These are stable API mechanisms. Neither establishes a complete transactional rollback guarantee at every possible interruption *inside* CreateProcessW; do not invent one. No crash-injection, inherited-handle enumeration or Windows process-lifetime test was performed.

## What the researcher missed

- **Clipboard history and cloud sync are part of the privacy boundary.** [Microsoft Clipboard Formats](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats), primary, updated 2020-08-19, documents `ExcludeClipboardContentFromMonitorProcessing`, `CanIncludeInClipboardHistory=0` and `CanUploadToCloudClipboard=0`. Temporary transcript writes can otherwise enter history/sync before restoration. Apply the chosen exclusion method on temporary writes and preserve privacy markers when restoring prior data; test with history and cloud sync enabled. These formats are controls for the Windows features, not a confidentiality guarantee against other clipboard readers. The source is old but explicit and still present in current live docs; no propagation timing or universal third-party-manager compliance was tested.
- **Suspended creation is not enough to prevent all orphans.** F2.12's corrected creation-time job assignment is material. Also distinguish job *membership* inheritance from inheriting a job *handle*. The latter can keep the last-handle trigger from firing; use explicit IPC inheritance and test nested launchers, forced UI exit, assignment failures and child/grandchild behavior.
- **Hover activation needs its own guard.** The no-activate style does not cover the active-window-tracking setting. Include `WM_MOUSEACTIVATE` handling and pointer/touch/drag tests as well as show/move tests.
- **UIA itself can block.** [UI Automation Threading Issues](https://learn.microsoft.com/en-us/dotnet/framework/ui-automation/ui-automation-threading-issues), primary, updated 2021-09-15, warns of poor responsiveness/deadlocks when desktop-wide clients call UIA from their UI thread. [IUIAutomation2 connection timeout](https://learn.microsoft.com/en-us/windows/win32/api/uiautomationclient/nf-uiautomationclient-iuiautomation2-get_connectiontimeout), primary, repository metadata 2018-12-05, exposes bounded provider waits. Use a separate bounded inspection path and stale-result/generation checks; do not let an unresponsive editor block the key hook, recording stop, overlay, cancellation or quit. A timeout setting is not proof that every COM/provider call is promptly cancellable.
- **Capture silence must be represented explicitly.** The independent [Microsoft capture source](https://github.com/microsoft/Windows-classic-samples/blob/8f31b1ff79d669b4ba9609f2640635b3b8a9e0a4/Samples/Win7Samples/multimedia/audio/CaptureSharedEventDriven/WASAPICapture.cpp) handles `AUDCLNT_BUFFERFLAGS_SILENT` by writing silence instead of copying packet data. This needs coverage alongside discontinuity/timestamp flags, packet draining, buffer lifetime and device changes; it matters for safe resampler inputs and false speech. This source is historical implementation guidance, not a measured modern-driver result.
- **A truly local inference claim must distinguish downstream text delivery.** Even with model network access disabled, a user-selected browser/editor, OS clipboard sync or third-party clipboard manager can process the inserted/copied text. Product wording and tests must describe the application's own handling and avoid claiming control of the destination app's networking.

## Remaining validation limits

- Exact standalone Fn behavior, hook suppression/coexistence, injected-event marking, layout/AltGr interactions and lost-release recovery remain hardware/runtime tests.
- The independently checked source contracts do not establish microphone privacy permission UX, Bluetooth format stability or recovery success in this app.
- Unicode delivery has no verified universal target-level success acknowledgment; focus revalidation reduces risk but does not make delivery atomic.
- Media capability enforcement for the exact chosen deployment and reliable pause/resume behavior remain unmeasured. The ownership race is correctly retained as an API limitation.
- No source here proves model RAM/VRAM reclamation deadlines after UI close or crash; process, GPU context and handle-lifetime measurements remain release gates.
