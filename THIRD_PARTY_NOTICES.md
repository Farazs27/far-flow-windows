# Third-party materials and evaluated dependencies

The root rights reservation does not replace third-party terms.

## Included in this planning repository

Zalando Sans — Copyright 2025 The Zalando Sans Project Authors, SIL Open Font License 1.1. Full notice: [brand/fonts/ZalandoSans-OFL.txt](brand/fonts/ZalandoSans-OFL.txt). Upstream: https://github.com/zalando/sans. The font has not been renamed or modified.

The flowing F, app icon PNG and travertine artwork are Far Flow brand assets reused with the product owner's authorization. Windows system symbols and Apple SF Symbols/sounds are not included as redistribution assets.

## Evaluated, not bundled runtime dependencies

The research evaluates WinUI/Windows App SDK, .NET, sherpa-onnx/ONNX Runtime, whisper.cpp, llama.cpp, Parakeet v3, Whisper and Qwen model derivatives. No native inference binaries or model weights are included here. Candidate code/model licenses and derivative attribution must be verified again at the exact revisions selected during the Windows feasibility phase; original model terms do not disappear after conversion/quantization.

See [the model plan](docs/COMPATIBILITY-AND-MODELS.md) and [validated research](research/30-final-report.md). A release must add exact license/notice texts and an SBOM for the actual shipped dependency graph rather than treating this evaluation list as a complete runtime notice bundle.
