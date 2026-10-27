# Test Far Flow on Windows

**Phase 1 preview — Windows 11 x64, Intel/AMD. CPU transcription.** No subscription, account, API key or Python installation is needed. This is an early test build, not the finished Mac feature set. The preview is unsigned; verify its repository origin and SHA-256 checksum before running it. Production signing and installation are later release gates.

1. Download the ZIP from [GitHub Releases](https://github.com/Farazs27/far-flow-windows/releases). Right-click it and choose **Extract All**. Keep the whole extracted folder together.
2. Open **FarFlow.Windows.exe**. Do not run as administrator. The `ModelHost` subfolder must remain beside it.
3. Open **Local model → Download local model**. Approve the one-time **670 MB** download, then wait for **Ready**. The speech model is stored in `%LOCALAPPDATA%\FarFlowWindows\Models\parakeet-v3-int8`, separately from the app. If copied from another PC, all four exact files must pass the app's built-in hash checks.
4. In Windows Settings → Privacy & security → Microphone, enable microphone access and desktop-app access. Select your input device in Sound settings.
5. Open **Notepad** and click its document. Hold **F8**, speak, then release. Keep focus there until your words arrive. Double-tap F8 for hands-free; press it once again to finish. Esc cancels. Record here and Copy text also work inside Far Flow.
6. Disconnect the internet and repeat. Normal dictation does not need a connection. Only the explicit model-download button uses the network; documentation links open your browser.

F8 is reserved while this preview is open; use the keyboard's Fn modifier if necessary to produce an ordinary F8 event. Recordings are limited to 90 seconds. The tiny waveform bar can be dragged. Closing the main window quits Far Flow and its model worker; minimize keeps them available. Transcripts are session-only, and audio is not written to disk.

## Please check these cases

| Test | Expected result |
| --- | --- |
| English, 1–2 sentences | Recognizable English text; no translation |
| Dutch, 1–2 sentences | Dutch words and punctuation; no translation |
| Mixed English/Dutch | Both languages retained; report any errors |
| Hold/release and double-tap | One recording; hands-free stays on until stopped |
| Silent recording | No invented text |
| Switch away from Notepad before processing finishes | Transcript retained for Copy, no automatic refocus |
| Another app or an elevated app | Copy recovery; direct insertion is limited to ordinary Notepad in Phase 1 |
| Internet disconnected after model installation | Dictation still works |
| Close the main window | No FarFlow.ModelHost process remains in Task Manager |
| Restart without downloading again | Local model verifies and loads |

Tell the owner your Windows version, CPU, RAM, GPU, microphone type, whether the test was offline, and the after-stop time displayed in the app. Use harmless example sentences. Do not upload private recordings, personal dictation or full machine diagnostics to a public issue.

## Preview limits

GPU acceleration, the Parakeet-versus-Whisper quality bake-off, broad editor insertion, media pause/resume, configurable shortcuts, persistent history, dictionary, snippets and writing tools remain later work. No claim is made that every Windows PC or microphone is already certified. Physical microphone, Bluetooth, MSI/RTX hardware and personal-voice results require real tester feedback.

The app writes a local `startup-error.txt` only if startup fails. It does not upload this file. Model removal is manual in this preview: quit the app, then remove its model folder. Deleting the extracted app folder removes the app itself.
