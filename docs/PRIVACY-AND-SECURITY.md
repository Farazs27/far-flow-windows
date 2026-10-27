# Proposed privacy and security design

> Planning/release-contract snapshot. See [Phase 1 execution and current evidence](PHASE-1-STATUS.md) for the implemented preview and the tests actually run.

Status: architecture requirements, not a claim of implemented/tested Windows controls. Research supports the API boundaries; Windows enforcement and offline behavior must be tested.

## Local processing boundary

No required login, paid inference API, remote transcription, analytics endpoint or cloud writing service. Models are explicitly installed/imported and then loaded by local path. The product must not call auto-provisioning runtime APIs during transcription or start an unauthenticated localhost inference service.

A network-denied test covers ASR, cleanup, transforms, history and editing after installation. Package/dependency/model downloads are separate deliberate operations. Windows OS services, the browser/editor receiving the inserted text, enterprise monitoring, backups and other clipboard readers are outside the local inference boundary; do not market those as controlled by Far Flow.

## Data flow and retention

Audio is bounded in memory and discarded after completion/cancellation; no always-listening buffer or background audio service. Retained raw/cleaned text obeys Off/24-hour/30-day/Keep settings. Notes/dictionary/snippets/preferences are separate user data. Package data paths are resolved by the selected app model; model/data removal behavior is documented and tested across update/uninstall.

SQLite is not automatically encrypted by choosing WinUI/MSIX. Design the first release around per-user OS ACLs and existing device encryption; do not claim a separately encrypted database unless implemented and tested. A JSON export is deliberately portable plaintext and remains wherever the user saves it. Imports create a pre-import backup and validate size/schema/fields before replacing collections. Real dictations, exports, recordings and diagnostics are never public repository fixtures.

Diagnostics default to state/error codes, model/runtime version and anonymized timing rather than transcript/audio/editor contents. An explicit user export can include reviewed diagnostics; crash dumps can contain process memory and must remain local unless the user deliberately reviews/shares them. Do not place transcript strings in process command-line arguments.

## Desktop integration boundaries

The normal app runs as the signed-in user. Do not default to administrator or UIAccess. Known password/protected fields and secure desktops block automatic capture/delivery; uncertain targets use recovery instead of promising universal detection. UIA is inspected off the UI/hook thread with a bounded strategy because providers can block. Target identity/caret is frozen at stop, revalidated before delivery, and never resurrected by forcing another window into the foreground.

Clipboard fallback is deliberate and compatibility-tested. Temporary transcript data is excluded from Windows history/cloud synchronization using the documented formats/options. Preserve the original item's privacy/exclusion metadata when restoring, and avoid creating a new historical/synced entry through restoration. Restore only when Far Flow still owns the expected sequence under the clipboard lock; any newer user copy wins. If lossless handling of a delayed/private format cannot be established, decline automatic substitution and offer explicit Copy. Other clipboard readers may still observe temporary data.

No Enter is appended to terminal text, and multiline terminal content is not pasted automatically: existing embedded newlines can execute commands without an added Enter. AI-generated text is data for reviewed writing, never executable instructions.

## Trusted native/model artifacts

Each selected model/runtime has an immutable upstream revision, exact expected filenames/sizes/hashes, original license/derivative attribution and a tested compatibility profile. Verify bytes before activation, keep downloads staged, reject path traversal and canonical-path escapes, and preserve the previous usable installation on failure. Metadata LFS hashes are research evidence until actual downloaded bytes are recomputed; small non-LFS files still need hashes.

A local GGUF/ONNX file exercises native parsers. A model's hash proves identity against the trusted manifest, not safety against every runtime vulnerability. Do not enable arbitrary executable/model imports or remote custom model code. Load native libraries only from approved application/backend locations with a closed DLL search policy; do not search the current working directory or a globally writable path. Track upstream security advisories and rebuild pinned native dependencies as needed.

The model helper process is useful for failure/lifetime isolation but remains an ordinary user process unless a separately designed restricted-token/AppContainer boundary is added. Job Objects alone are not a malicious-model sandbox. IPC is local/current-user-scoped, bounded/versioned, per-launch named and session-ID checked; no loopback server is required.

## Process ownership and distribution

The main app is the sole owner of the noninherited kill-on-close job handle. Use creation-time worker assignment and an explicit inherited-handle allowlist. Main-window close stops capture/hooks/delivery, resolves still-owned media pause, requests graceful model shutdown and enforces a bounded fallback; minimize is distinct.

No certificate, PFX, password or signing token belongs in Git, binaries or logs. Production signing route depends on the real publisher identity and eligibility; do not assume an individual publisher is eligible for a service whose current individual eligibility is US/Canada. Signing does not guarantee immediate SmartScreen reputation or permission to access protected targets. Signing purchase/provisioning/trust changes are future explicit owner actions, not part of this research run.
