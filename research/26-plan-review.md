# Final plan review

Reviewed 2026-10-27: `docs/IMPLEMENTATION-PLAN.md`, 98-line draft, against dossiers 1–4 and validations 1–4. Review was read-only; no Windows execution evidence was created. The companion architecture/model/test documents were still being drafted and were not treated as missing deliverables.

**No release-blocking factual error found in the proposed architecture.** The following two scope/safety details should be made explicit before implementation sign-off; neither requires changing the chosen stack.

## Corrections / sign-off conditions

1. **P2 — Scope the full-backup-only import rule to migration.** At line 73, “Import user data only from deliberate schema-1 full JSON exports with ISO 8601 dates” is broader than the audited existing behavior. Validation 4, F4.4 establishes a separate dictionary-only array import/export format as well as full UserData backup JSON. If the sentence governs every import, it silently excludes that existing workflow. Narrow it to “For Mac-to-Windows full-data migration…” and give dictionary-only import/export its own format decision/fixtures in the parity contract. Do not route dictionary arrays through the full-backup parser or imply all Mac JSON uses the same envelope.

2. **P2 — Clipboard restoration needs a privacy-preservation contract too.** At line 86, “mark temporary data excluded from history/sync, preserve later user copies” covers the temporary transcript write but does not state what happens to original privacy markers during restoration. Validation 2's missed-angle evidence documents Windows history/cloud exclusion formats. The companion insertion contract and tests should preserve supported original privacy markers and prevent programmatic restoration from creating an unintended history/sync item. A successful sequence-number check protects against overwriting a later copy; it does not preserve those markers or prevent prior capture by clipboard readers. Retain the plan's explicit limitation about other clipboard readers.

## Load-bearing points checked without a correction

- Lines 19–23 preserve Parakeet v3 versus English-only v2 and exact Qwen3-4B-Instruct-2507 versus generic Qwen3 identity. They make CPU/GPU quality, effective provider, ABI/dependencies and memory fit measurement gates, not achieved facts. The approximate 670 MB is the uncompressed selected int8 core pack, consistent with validation 1.
- Lines 28 and 31 correctly keep Fn hardware-dependent and capture the final target at **stop/release**, preserving the audited Mac timing. The companion gesture contract must retain immediate first-key-down recording; the 0.22-second threshold is not a startup delay. Target/caret continuity and safe failure remain acceptance requirements.
- Lines 45 and 87 incorporate creation-time Job Object assignment, exclude inherited job handles and avoid claiming the job is a malicious-model sandbox. No broader interrupted-CreateProcess transactional guarantee is promised.
- Lines 13–15, 63–67 and 96 keep support provisional, distinguish CPU-only and NVIDIA test lanes, and do not relabel Mac/CI evidence as Windows certification. The 25H2/26H2 priority matches validation 3's current qualified support conclusion.
- Lines 73–75 and 90 keep original rights separate from third-party terms, signing identity/eligibility unresolved, full-trust MSIX conditional, single-file uncommitted and clean-machine installation a gate. No SmartScreen-silence or universal no-admin result is promised.

No cosmetic or framework-preference changes requested. Exact dependency locks, shipped notices, release signatures and hardware results still require the later evidence gates already stated in the plan.
