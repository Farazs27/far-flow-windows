# Third-party notices

Original Far Flow application code, documentation and branding remain all rights reserved. These terms do not replace the licenses of third-party components. Published previews are intended for owner-invited evaluation.

| Included component | Version / origin | Terms |
| --- | --- | --- |
| Microsoft WinUI template | Microsoft.WindowsAppSDK.WinUI.CSharp.Templates 0.0.7-alpha | Microsoft MIT; [license](licenses/WindowsAppSDK-MIT.txt) |
| Windows App SDK / WinUI | Microsoft.WindowsAppSDK 2.5.1 | Microsoft MIT; Microsoft redistributable component terms apply where supplied |
| .NET | SDK 10.0.403, self-contained .NET 10 runtime selected by that SDK | MIT and component-specific licenses; runtime license files remain in the published folder |
| NAudio | 2.2.1, Mark Heath and contributors | [MIT](licenses/NAudio-MIT.txt) |
| sherpa-onnx | org.k2fsa.sherpa.onnx 2.0.1, commit af2d8d81dbfa61c4cea4c568da78f3982edca005; Xiaomi / next-gen Kaldi contributors | [Apache 2.0](licenses/Sherpa-ONNX-Apache-2.0.txt) |
| ONNX Runtime and its dependencies | Native runtime in the sherpa-onnx Windows package | [MIT](licenses/ONNXRuntime-MIT.txt), [upstream third-party notices](licenses/ONNXRuntime-ThirdPartyNotices.txt) |
| Microsoft Visual C++ runtime | Official x64 redistributable CRT from the build toolchain; version recorded with build evidence | Microsoft Visual Studio redistributable terms; not governed by Far Flow's license |
| Zalando Sans | Approved font source carried from the Mac edition | [SIL Open Font License 1.1](licenses/ZalandoSans-OFL.txt) |

## Separately downloaded speech model

**NVIDIA Parakeet TDT 0.6B v3**, by NVIDIA, is licensed under [Creative Commons Attribution 4.0](https://creativecommons.org/licenses/by/4.0/). [Original model and model card](https://huggingface.co/nvidia/parakeet-tdt-0.6b-v3). Far Flow downloads a format/precision adaptation, the int8 ONNX conversion by **csukuangfj**, from [the pinned conversion revision](https://huggingface.co/csukuangfj/sherpa-onnx-nemo-parakeet-tdt-0.6b-v3-int8/tree/2bda32ec70b097a55adaa07d9a7173915b43cc78). Conversion and quantization change the representation and precision. No endorsement by NVIDIA or the conversion author is implied. Model weights are not included in the repository or preview ZIP. The installer writes attribution beside the locally installed files.

Public upstream English/Dutch speech fixtures are fetched by CI at fixed revisions and verified by hash. They are not redistributed in app releases. Fixture provenance is recorded in `scripts/smoke-speech.ps1`. The model/runtimes still require the planned broader personal-voice evaluation. Whisper, Qwen, llama.cpp and GPU-provider candidates discussed in research are not bundled in this preview.
