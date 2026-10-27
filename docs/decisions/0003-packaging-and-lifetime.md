# ADR 0003 — Packaging and owned lifetime
Status: PROPOSED, deployment-spike gate, 27 October 2026.

Primary delivery proposal: full-trust MSIX x64 with explicit publisher identity, tested microphone/global-media declarations and required runtime closure. Models/data live outside the signed executable payload. A per-user unpackaged installer is the deliberate alternative if a documented native requirement cannot pass in the selected package configuration.

Full-trust MSIX is not the UWP AppContainer/suspension model. Do not assume packaging prevents desktop hooks, or that every WinUI launch has identical capability/update behavior. Build and exercise the actual packaged app as a standard user.

Include both .NET and Windows App SDK runtime dependencies when choosing self-contained deployment; native inference DLLs must also be accounted for. Current Microsoft single-file guides conflict and all-content extraction has compatibility/security/performance caveats. A tested signed package/folder installer is the baseline; no universal single-file promise.

Main-window close and Quit stop all Far Flow work. Minimize keeps the app ready. Use creation-time Job Object worker assignment and a noninherited sole-owner handle, not a spawn/late-assign sequence with an orphan window. This controls lifetime/crash containment, not malicious-model permissions.

Signing/update route is not selected solely from geography: identity/eligibility, certificate trust, install/update/rollback and SmartScreen behavior require evidence and owner provisioning. Source publication does not create a signed Windows release. Sources/corrections: F2.12, F3.5–F3.10 in [the report](../../research/30-final-report.md).
