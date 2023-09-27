# ResonitePTTKeybinds
A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that can disable or remap the Push to Talk / Mute keybinds on Mouse5, M, and V. Set remapped keys using [ResoniteModSettings](https://github.com/badhaloninja/ResoniteModSettings) in-game. Additionally allows setting keybinds to set your current Voicemode.

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [ResonitePTTKeybinds.dll](https://github.com/XDelta/ResonitePTTKeybinds/releases/latest/download/ResonitePTTKeybinds.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs.


## Config Options

| Config Option     | Default | Description |
| ------------------ | ------- | ----------- |
| `disableMuteKey` | `false` | Disables the Mute toggle keybind M  (requires restart) |
| `disablePTTKeys` | `false` | Disables the PTT keybind Mouse5 and V  (requires restart) |
| `remapKeys` | `false` | If `true` will disable the default PTT/Mute keys listed above and use the 'custom' Keys defined below (requires restart) |
| `customMuteKey` | (M) | Set custom Mute (toggle) keybind, set to 0 (None) to disable (requires restart) |
| `customPTTKey` | (V) | Set custom PTT keybind, set to 0 (None) to disable (requires restart) |
| `MuteKey` | (None) | Set Mute Mode keybind |
| `WhisperKey` | (None) | Set Whisper Mode keybind |
| `NormalKey` | (None) | Set Normal Mode keybind |
| `ShoutKey` | (None) | Set Shout Mode keybind |
| `BroadcastKey` | (None) | Set Broadcast Mode keybind |
