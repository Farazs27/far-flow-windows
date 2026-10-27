# Dossier 1: Local Windows ASR and optional writing inference

Researcher scope: maintained engines, exact model/artifact identity, language coverage, Windows CPU/GPU support and inference security | Goal context: approval-ready separate native Windows Far Flow architecture and implementation plan | Date: 2026-10-27

This dossier records source evidence, not a model selection or benchmark result. No models were executed, no Windows device was available, and no model weights were downloaded. Read-only publisher metadata is preserved in `inference-artifact-evidence.json`; its SHA-256 values are publisher/LFS metadata, not locally verified downloads.

## Search log

- `site.huggingface.co nvidia parakeet-tdt-0.6b-v3 25 languages Dutch license` → NVIDIA model card, language list and governing terms.
- `site.k2-fsa.github.io sherpa onnx Windows C# Parakeet v3 execution provider` → C# bindings, Windows builds and model conversion docs.
- `site.github.com ggml-org whisper.cpp Windows Vulkan CUDA turbo multilingual` → current upstream source README and releases.
- `site.huggingface.co Qwen Qwen3 GGUF license llama.cpp CPU RAM` → official Qwen GGUF models and publisher local deployment docs.
- **Disconfirming:** `site.github.com/k2-fsa/sherpa-onnx/issues parakeet v3 CUDA int8 DirectML` → a real model/runtime opset failure, not evidence of universal accelerated compatibility.
- **Disconfirming:** `site.github.com/k2-fsa/sherpa-onnx "DirectML" "parakeet"` and inspection of the tagged provider/CMake source → provider availability is build-dependent; DirectML default runtime and offline TensorRT restrictions complicate a generic GPU claim.
- `site.onnxruntime.ai quantization "CPU" "GPU" "TensorRT" "dynamic"` → quantized operator/backend and hardware-dependent limits, including potential regressions on old CPUs.
- `site.huggingface.co/nvidia/parakeet-tdt-0.6b-v3 "code" "switch"` → did not locate a Dutch/English same-utterance code-switch evaluation.
- `site.huggingface.co Qwen3-ASR Dutch languages` → newer official Dutch-capable ASR candidate; confirmed current sherpa C# API and ONNX asset documentation rather than assuming only Parakeet and Whisper exist.
- `site.huggingface.co/Qwen "Qwen3.5-4B" "license"` → newer Qwen3.5 family exists; inspected runtime-publisher small GGUF listing without assuming better writing quality.
- Direct GitHub release APIs, immutable tag commits, HF model APIs and immutable revision file trees → exact versions, artifact sizes, hashes, model-card metadata and absent licensing files.

## Findings

### F1.1 — Parakeet v3 advertises Dutch among 25 languages

- **Claim:** NVIDIA's Parakeet TDT 0.6B v3 model card explicitly includes Dutch and English among 25 supported European languages and describes automatic language detection.
- **Source:** https://huggingface.co/nvidia/parakeet-tdt-0.6b-v3 — primary/vendor model card — declared model release 2025-08-14; current repository revision `541d1f99c6b0c3cd0b11a95167540bb8edefd82b`, modified 2026-08-05; accessed 2026-10-27.
- **Confidence:** High for advertised language coverage; unmeasured for Far Flow accuracy/latency.
- **Notes:** Governing license is CC-BY-4.0. Repository creation was 2025-08-04, which differs from the card's declared release date; creation is not substituted for release. Automatic language detection is not a validated guarantee of Dutch/English switching inside one sentence. The card's throughput/long-audio claims concern its stated evaluation environment, including A100 hardware, rather than unspecified Intel/AMD PCs or an RTX 3070-class PC. A contradictory current NVIDIA checkpoint table labels v3 English: https://docs.nvidia.com/nemo/speech/nightly/asr/asr_checkpoints.html (official, undated living docs, accessed 2026-10-27). That table conflicts with the model-specific card and metadata and must not be silently treated as the correct language inventory.

### F1.2 — Parakeet v2 is an English model

- **Claim:** NVIDIA's Parakeet TDT 0.6B v2 model card describes English transcription, so v2 is not documentary evidence for Dutch support.
- **Source:** https://huggingface.co/nvidia/parakeet-tdt-0.6b-v2 — primary/vendor model card — declared release 2025-05-01; current revision `ae9ad07059c7c739ffaf932226a8fe64ae2620b0`, modified 2026-06-29; accessed 2026-10-27.
- **Confidence:** High.
- **Notes:** The card separately points readers to multilingual v3 and declares CC-BY-4.0. The named v2/v3 variants cannot be interchanged based on the word “Parakeet.” The reported v2 RTFx uses batch size 128; it is not a measurement of one dictation ending on an ordinary Windows CPU.

### F1.3 — The sherpa Parakeet v3 export requires a complete model bundle

- **Claim:** The published int8 Parakeet v3 ONNX bundle used by sherpa contains separate encoder, decoder, joiner and token-table files, rather than a single interchangeable model file.
- **Source:** https://k2-fsa.github.io/sherpa/onnx/pretrained_models/offline-transducer/nemo-transducer-models.html — primary/maintainer documentation — undated living docs, accessed 2026-10-27; https://huggingface.co/api/models/csukuangfj/sherpa-onnx-nemo-parakeet-tdt-0.6b-v3-int8/tree/2bda32ec70b097a55adaa07d9a7173915b43cc78?recursive=true&expand=false — primary/maintainer artifact listing — revision dated 2025-08-16.
- **Confidence:** High for artifact contents; Medium for exact derivative notice completeness.
- **Notes:** At that revision the files are `encoder.int8.onnx` (652,184,281 bytes), `decoder.int8.onnx` (11,845,275 bytes), `joiner.int8.onnx` (6,355,277 bytes) and `tokens.txt` (93,939 bytes): 670,478,772 bytes in total, excluding test audio and packaging. The FP32 companion revision `1a468a35cbba69418f126de829e75261dea4a4e4` additionally requires `encoder.weights` (2,435,420,160 bytes) beside `encoder.onnx`; forgetting external tensor data breaks a supposedly complete install. The int8 HF export has no README/model card or LICENSE file. Its derivation is documented upstream, but the artifact's redistribution notice must explicitly retain NVIDIA's CC-BY-4.0 terms and identify the conversion; the sherpa Apache license alone does not describe the source weights. No internet retrieval is required by the documented recognition invocation after local files exist.

### F1.4 — Maintained sherpa C# packages wrap native Windows binaries

- **Claim:** sherpa-onnx publishes a managed C# API that calls native libraries and provides Windows runtime packaging, rather than requiring Python in the deployed application.
- **Source:** https://k2-fsa.github.io/sherpa/onnx/csharp-api/index.html — primary/maintainer documentation — undated, accessed 2026-10-27; https://github.com/k2-fsa/sherpa-onnx/blob/v2.0.1/scripts/dotnet/sherpa-onnx.csproj.in and https://github.com/k2-fsa/sherpa-onnx/blob/v2.0.1/scripts/dotnet/OfflineRecognizer.cs — primary/tagged source — v2.0.1, released 2026-10-26; https://www.nuget.org/packages/org.k2fsa.sherpa.onnx/2.0.1 — primary/package registry — version 2.0.1, accessed 2026-10-27.
- **Confidence:** High for the binding/package path; untested for the proposed app.
- **Notes:** Tagged source declares `win-x64`, `win-x86`, `win-arm64` runtime identifiers, Apache-2.0 package licensing, and `DllImport` calls. Registry API confirms 2.0.1 exists. The source release includes a Windows x64 shared MD Release no-TTS archive (20,962,760 bytes) and separate Windows CUDA archives, so CPU, accelerator and architecture packages are distinguishable. Source package frameworks include .NET 8 and .NET 10, which establishes compatibility targets, not a recommendation to use unsupported historical framework targets. Upstream docs use wildcard package examples; reproducibility requires an explicit chosen version instead. C# NuGet GPU capability was not established from its name alone.

### F1.5 — sherpa provider selection is conditional, and offline TensorRT is restricted

- **Claim:** sherpa's v2.0.1 execution-provider code can fall back to CPU when requested acceleration is unavailable, so a requested provider is not proof that a particular model graph ran on that GPU.
- **Source:** https://github.com/k2-fsa/sherpa-onnx/blob/v2.0.1/sherpa-onnx/csrc/session.cc — primary/tagged source — released 2026-10-26; https://github.com/k2-fsa/sherpa-onnx/blob/v2.0.1/cmake/onnxruntime-win-x64-directml.cmake — primary/tagged build source — released 2026-10-26.
- **Confidence:** High for source behavior; Medium for inference about any specific bundled model/provider combination.
- **Notes:** CPU, CUDA and conditional Windows DirectML branches are present. CUDA failure paths explicitly log CPU fallback. TensorRT's branch explicitly rejects the unsupported offline path when provider configuration is absent; it says online models only. Parakeet v3 here is an offline transducer, so no supported TensorRT path was established. DirectML requires a separate build flag and Windows x64 shared-library build; its default CMake download is ORT DirectML 1.14.1 plus DirectML 1.15.0 unless preinstalled runtime paths override it. Current CUDA release artifacts name ORT 1.28.2/CUDA 12.x or 13.x/cuDNN 9.x. These differences need explicit per-artifact validation; I did not find an authoritative Parakeet-v3-int8 × Windows CUDA/DirectML performance/support matrix. ONNX Runtime's separate operator placement may retain CPU work even if GPU provider initialization succeeds: https://huggingface.co/docs/optimum-onnx/main/en/onnxruntime/usage_guides/gpu (primary/library documentation, undated, accessed 2026-10-27).

### F1.6 — Whisper supplies multilingual small and turbo artifacts

- **Claim:** Multilingual Whisper small and large-v3-turbo artifacts, including quantized variants, are published for whisper.cpp.
- **Source:** https://huggingface.co/ggerganov/whisper.cpp/tree/5359861c739e955e79d9a303bcbc70fb988958b1 — primary/runtime-maintainer artifact listing — revision modified 2024-10-29; https://github.com/openai/whisper/blob/main/README.md — primary/model-author documentation — undated living README, accessed 2026-10-27; https://github.com/openai/whisper/blob/main/whisper/tokenizer.py — primary/model-author language inventory — accessed 2026-10-27.
- **Confidence:** High for artifact existence and documented multilingual capability; unmeasured for target quality.
- **Notes:** Use `small`, rather than English-only `small.en`, for the documented multilingual path. Dutch is present in the tokenizer's language map. File sizes: `ggml-small.bin` 487,601,967 bytes; `ggml-small-q5_1.bin` 190,085,487 bytes; `ggml-large-v3-turbo.bin` 1,624,555,275 bytes; `ggml-large-v3-turbo-q5_0.bin` 574,041,195 bytes. OpenAI explicitly licenses code and weights under MIT; the maintainer's HF artifact metadata also declares MIT. Turbo is not trained for translating non-English speech into English; transcription and translation should not be conflated. The author's published relative-speed table measures English on A100, not CPU-only Windows or RTX 3070. No Far Flow mixed-language score was found.

### F1.7 — whisper.cpp has Windows CPU, CUDA and Vulkan build paths

- **Claim:** whisper.cpp v1.9.5 documents Windows support and distinct CPU-only, NVIDIA CUDA and cross-vendor Vulkan inference build paths.
- **Source:** https://github.com/ggml-org/whisper.cpp/blob/v1.9.5/README.md — primary/tagged upstream documentation — released 2026-10-06; https://github.com/ggml-org/whisper.cpp/releases/tag/v1.9.5 — primary/upstream release — 2026-10-06.
- **Confidence:** High for advertised platform/build capability; no speed claim.
- **Notes:** README declares MIT code licensing, x86 AVX support and separate `GGML_CUDA` / `GGML_VULKAN` build flags. Vulkan requires a compatible driver. “Cross-vendor” is not evidence that every Intel/AMD iGPU or old CPU reaches dictation latency targets. Quantization's benefit is hardware-dependent. The semantic release's GitHub API returned no downloadable binaries; its release text points to nightly `b5454`. A shipping build must record the actual source commit/binary provenance rather than assuming a version-labelled Windows archive exists. v1.9.5 commit: `d1be6fde11ac6e0407606b4e42fe72d34add8037`. No Windows runtime, driver or application integration was exercised here.

### F1.8 — Core ML model artifacts are platform-specific

- **Claim:** whisper.cpp's Core ML encoder path is explicitly an Apple Silicon/Apple Neural Engine path, so its compiled `.mlmodelc` artifact is not the documented Windows CPU/CUDA/Vulkan model input.
- **Source:** https://github.com/ggml-org/whisper.cpp/blob/v1.9.5/README.md#core-ml-support — primary/tagged upstream documentation — released 2026-10-06; https://github.com/ggml-org/whisper.cpp/blob/v1.9.5/models/convert-pt-to-ggml.py — primary/tagged conversion format source — released 2026-10-06.
- **Confidence:** High for Core ML scope; Windows migration implication is an explicit inference.
- **Notes:** The conversion script documents a single custom GGML binary carrying hyperparameters, mel filters, vocabulary and weights. This is distinct from the optional Apple encoder cache. The corresponding sherpa Windows path consumes ONNX plus tokens; the writing runtime's documented path consumes GGUF. Existing Mac Core ML/MLX bundles cannot be declared ready-to-load Windows packages merely because the underlying model family/name matches. MLX-format conversion parity for the actual Far Flow Mac writing model was not tested, and this finding does not assert that every safetensors source file is unusable as conversion input.

### F1.9 — Official Qwen GGUF writing candidates have exact published identities

- **Claim:** Qwen publishes Apache-2.0 Qwen3-4B and Qwen3-8B GGUF writing-model artifacts suitable for evaluation with llama.cpp.
- **Source:** https://huggingface.co/Qwen/Qwen3-4B-GGUF — primary/vendor model card — repository created 2025-05-05, modified 2025-05-21; https://huggingface.co/Qwen/Qwen3-8B-GGUF — primary/vendor model card — repository created 2025-05-03, modified 2025-05-21; https://qwen.readthedocs.io/en/stable/run_locally/llama.cpp.html — primary/vendor deployment documentation — undated, accessed 2026-10-27.
- **Confidence:** High for artifacts/license and documented runtime use; no writing-quality or latency guarantee.
- **Notes:** Verified the actual 4B artifact repository's LICENSE text, not only a catalog badge. Q4_K_M file sizes are 2,497,280,256 bytes (4B) and 5,027,783,488 bytes (8B); exact revisions/hashes are in the evidence JSON. Vendor docs describe GGUF as containing model data including tokenizer and generation configuration, supporting offline loading after download. Qwen3 supports a non-thinking mode, which must be configured/tested for direct short writing transforms rather than assuming a prose-only response. No Dutch preservation, instruction-following, factual fidelity or safe transformation acceptance was measured. Newer Qwen3.5 exists; the runtime publisher also offers `ggml-org/Qwen3.5-0.8B-GGUF` Q4_0 (563,036,064 bytes, Apache-2.0 metadata, revision 2026-10-19), but the available tiny file is not evidence of writing parity with 4B/8B.

### F1.10 — Llama memory fit depends on context and buffers beyond weights

- **Claim:** llama.cpp's required runtime memory includes model weights, context/KV state and compute buffers, so a GGUF's disk size alone cannot establish a fit on a proposed GPU.
- **Source:** https://github.com/ggml-org/llama.cpp/discussions/9936 — primary/upstream maintainer explanation — maintainer reply 2024-10-21, follow-up 2025-02-03; https://github.com/ggml-org/llama.cpp/blob/v0.7.0/README.md — primary/tagged runtime documentation — released 2026-10-15; https://github.com/ggml-org/llama.cpp/blob/v0.7.0/docs/build.md — primary/tagged build documentation — released 2026-10-15.
- **Confidence:** High for allocation components and advertised capabilities; unverified numerical fit/latency on target PCs.
- **Notes:** Tagged README advertises x86 CPU intrinsics, CUDA, Vulkan and CPU/GPU hybrid offload. Windows x64 source builds are documented. Current build docs warn that `-ngl 0` alone can still use a GPU and specify `--device none` for complete GPU disabling. This matters when testing the CPU-only acceptance lane. The exact RTX 3070 desktop/laptop variant, VRAM, shared-memory behavior and available RAM were not supplied; no 4B/8B fit guarantee follows. Source license is MIT. Stable source `v0.7.0` commit is `620d2225cc436ab0b001e3200e590e3b286ef9ab`; its release assets are a nightly tag pointer rather than a complete Windows binary bundle. This is source-pin evidence, not a tested dependency freeze.

### F1.11 — Qwen3-ASR is an additional current Dutch-capable ASR candidate

- **Claim:** Qwen3-ASR-0.6B's official model card includes Dutch among its supported languages, providing an additional current candidate beyond Parakeet and Whisper.
- **Source:** https://huggingface.co/Qwen/Qwen3-ASR-0.6B — primary/vendor model card — repository created 2026-01-28, modified 2026-01-30, revision `5eb144179a02acc5e5ba31e748d22b0cf3e303b0`; https://k2-fsa.github.io/sherpa/onnx/qwen3-asr/pretrained.html — primary/maintainer export documentation — dated artifact `2026-03-25`, undated living page, accessed 2026-10-27; https://github.com/k2-fsa/sherpa-onnx/blob/v2.0.1/scripts/dotnet/OfflineQwen3AsrModelConfig.cs — primary/tagged C# API — released 2026-10-26.
- **Confidence:** High for advertised Dutch support and integration surface; unmeasured for target PCs.
- **Notes:** Official card declares Apache-2.0 and offline/streaming capability. sherpa's ONNX export docs list a convolution frontend, encoder, decoder, plus `tokenizer/merges.txt`, `tokenizer/vocab.json`, `tokenizer/tokenizer_config.json`; tokenizer assets must accompany the weights for offline use. Runtime/example “streaming” can mean VAD segmentation around a non-streaming export; do not equate vendor streaming claims with the semantics/latency of that particular ONNX export. I did not evaluate English/Dutch code-switch quality, native Windows performance, quantized accuracy or its precise GPU operator placement.

### F1.12 — Local models remain an input security boundary

- **Claim:** llama.cpp's upstream security policy treats untrusted model loading as a security boundary and calls for isolation and known-good artifact-hash verification.
- **Source:** https://github.com/ggml-org/llama.cpp/blob/v0.7.0/SECURITY.md — primary/tagged upstream security policy — released 2026-10-15; https://onnxruntime.ai/docs/#model-validation — primary/runtime security guidance — undated living docs, accessed 2026-10-27.
- **Confidence:** High for the upstream guidance; proposed Windows isolation strength not assessed.
- **Notes:** llama.cpp recommends sandboxing untrusted models and warns about exposing server/RPC functionality to untrusted networks. ONNX Runtime warns that malicious models may consume excessive compute/memory and recommends inspecting and testing untrusted models in a safe environment before production use. A separate helper process is a useful crash/lifetime boundary, but separation by itself is not proof of a sandbox or network denial. Download hashes verify identity/integrity against a trusted manifest, not model safety. No native runtime vulnerability scan, fuzzing, sandbox penetration test or offline-network audit was performed. Revisions below are observed reproducibility inputs; rolling security updates still need revalidation.

## Observed reproducibility inputs

These pins document exactly what was inspected, not a release-approved dependency selection. Binary, engine, model, tokenizer/external-data files, license notices and SHA-256 must be associated as one versioned model-pack manifest. See `inference-artifact-evidence.json` for full publisher metadata and all relevant LFS hashes.

| Component | Observed immutable identity | Publication / modification date |
|---|---|---|
| sherpa-onnx source | v2.0.1 / `af2d8d81dbfa61c4cea4c568da78f3982edca005` | Release 2026-10-26 |
| whisper.cpp source | v1.9.5 / `d1be6fde11ac6e0407606b4e42fe72d34add8037` | Release 2026-10-06 |
| llama.cpp source | v0.7.0 / `620d2225cc436ab0b001e3200e590e3b286ef9ab` | Release 2026-10-15 |
| Parakeet v3 int8 ONNX export | HF `2bda32ec70b097a55adaa07d9a7173915b43cc78` | 2025-08-16 |
| Parakeet v3 FP32 ONNX export | HF `1a468a35cbba69418f126de829e75261dea4a4e4` | 2025-08-16 |
| Whisper GGML models | HF `5359861c739e955e79d9a303bcbc70fb988958b1` | 2024-10-29 |
| Qwen3-4B-GGUF | HF `bc640142c66e1fdd12af0bd68f40445458f3869b` | 2025-05-21 |
| Qwen3-8B-GGUF | HF `7c41481f57cb95916b40956ab2f0b139b296d974` | 2025-05-21 |

Example publisher hashes: Parakeet v3 int8 encoder `acfc2b4456377e15d04f0243af540b7fe7c992f8d898d751cf134c3a55fd2247`; Whisper turbo Q5_0 `394221709cd5ad1f40c46e6031ca61bce88931e6e088c188294c6d5a55ffa7e2`; Qwen3-4B Q4_K_M `7485fe6f11af29433bc51cab58009521f205840f5b4ae3a32fa7f92e8534fdf5`. These were obtained from the immutable HF file tree APIs, not downloaded/recomputed. `tokens.txt` uses a Git blob oid in the tree rather than an LFS SHA-256; a complete production manifest needs an actual file SHA-256.

## Gaps — what I could not find

- No exact target Windows machine configuration or actual Windows hardware measurement. No claim that Intel, AMD, laptop 3070 and desktop 3070 have equivalent capability, memory or latency.
- No measured p50/p95 warm or cold dictation latency, total resident RAM/VRAM, concurrent ASR/writing budget, battery/thermal cost or model unload time for Far Flow on Windows.
- No authoritative Dutch/English same-utterance code-switch benchmark covering these exact ONNX/GGML artifacts, decoding options and Windows CPUs/GPUs. Per-language coverage and average WER do not resolve that question.
- No tested Parakeet-v3 graph × CPU/CUDA/DirectML/OpenVINO support/performance matrix, including numerical parity and actual operator placement. Generic ONNX provider lists are insufficient. FP32 and int8 must not be assumed equivalent acceleration paths.
- No conclusive minimum CPU ISA/RAM/OS specification for the entire proposed product. The build supports an architecture; that does not establish useful dictation latency on all machines in it.
- No complete third-party distribution notice in the Parakeet v3 converted HF artifact repository; only source-model governing license and conversion provenance were located. CUDA/driver/runtime redistribution details were not exhaustively audited.
- No established writing-quality threshold for English/Dutch style transformations or strong evidence that the tiny newer Qwen3.5 model preserves meaning as well as larger candidates. No benchmark was run to compare Qwen3 versus Qwen3.5.
- No direct reuse validation of the actual Mac MLX artifacts in a Windows runtime; conversion would need exact config/tokenizer/quantization inspection and parity testing.
- Semantic source releases and current nightly binary packaging differ for whisper.cpp and llama.cpp; no tested Windows binary dependency freeze exists yet.
- All offline claims depend on all required assets being installed. Initial model acquisition, update checking and runtime optional network surfaces were not traffic-audited.

## Disconfirming evidence check

- Tried to disprove “Parakeet v3 is Dutch-capable”: a current official NVIDIA summary table labels it English; the model-specific card, tokenizer/language metadata and sherpa's model-specific 25-language section disagree with that summary. Preserve the contradiction rather than repeat the summary table unquestioned.
- Tried to disprove “select CUDA/DirectML and every model accelerates”: tagged source contains explicit CPU fallback, offline TensorRT rejection and a separately configured DirectML runtime. An actual user report, https://github.com/k2-fsa/sherpa-onnx/issues/2842 (opened 2025-12-01; upstream issue, user-reported) records Parakeet v3 failing to load with an older ORT opset limit. This is evidence that version mismatch exists, not proof the current pinned build fails.
- Tried to disprove “quantized is always faster”: https://onnxruntime.ai/docs/performance/model-optimizations/quantization.html (official, undated living docs, accessed 2026-10-27) explicitly says hardware support/quantization overhead can make older devices slower; it also describes x86 U8S8 saturation/accuracy caveats. No old-CPU performance promise survives this check.
- Tried to disprove “GGUF fits because file size is below advertised VRAM”: upstream maintainer allocation explanations account for KV and compute buffers in addition to weights; exact simultaneous-model fit remains unmeasured.
- Tried to disprove “local inference makes downloaded files harmless”: both runtime security guidance sources advise treating model files/resource use as security inputs. Offline inference alone does not eliminate parser, resource exhaustion, input-injection or local exposure risks.
