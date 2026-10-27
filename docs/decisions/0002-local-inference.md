# ADR 0002 — Local inference with measured model profiles
Status: PROPOSED, benchmark-gated, 27 October 2026.

Decision: require a CPU-only path; evaluate Parakeet TDT v3 int8/sherpa as the leading ASR candidate against multilingual Whisper alternatives. Keep deterministic Light rules as the normal fast path. Offer a separately installed local 4B Instruct-2507 GGUF writer if provenance and quality/memory/latency gates pass.

GPU detection is not a GPU benchmark. CUDA/DirectML/Vulkan require matching native builds, graph/dtype/operator support, driver/runtime and actual effective-provider evidence. Do not assume an x64 CPU NuGet package contains a CUDA backend or that a quantized ONNX graph is fully accelerated. DirectML is not the initial cross-vendor promise.

Exact writer identity matters: original hybrid Qwen3-4B and non-thinking Qwen3-4B-Instruct-2507 are different checkpoints. Community GGUFs of the latter are evaluation candidates with pinned provenance, not official publisher conversions by assertion. Context/KV and compute buffers mean disk size does not prove a 3070 VRAM fit; 8B is not the default.

No weights are downloaded or installed by this planning repository. [Candidate metadata](../../planning/model-candidates.json) is not an install manifest. Select and hash the real runtime/model pack only after native/offline benchmark verification. See [the model policy](../COMPATIBILITY-AND-MODELS.md).
