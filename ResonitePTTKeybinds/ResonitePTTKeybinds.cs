using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using System;

namespace ResonitePTTKeybinds;
public class ResonitePTTKeybinds : ResoniteMod {
	public override string Name => "PTT Keybinds";
	public override string Author => "Delta";
	public override string Version => "1.4.0";
	public override string Link => "https://github.com/XDelta/ResonitePTTKeybinds";

	//TODO keycode config for rebinding activation keys
	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> disableMuteKey = new("disableMuteKey", "Disable the Mute toggle keybind M (requires restart)", () => false);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> disablePTTKeys = new("disableMousePTTKey", "Disable the PTT keybind Mouse5 and V (requires restart)", () => false);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> remapKeys = new("remapKeys", "Enable to remap keys, will disable all default PTT keys (requires restart)", () => false);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> customMuteKey = new("customMuteKey", "Set custom Mute (toggle) keybind, set to 0 (None) to disable (requires restart)", () => Key.M);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> customPTTKey = new("customPTTKey", "Set custom PTT keybind, set to 0 (None) to disable (requires restart)", () => Key.V);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> MuteKey = new("MuteKey", "Set Mute Mode keybind", () => Key.None);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> WhisperKey = new("WhisperKey", "Set Whisper Mode keybind", () => Key.None);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> NormalKey = new("NormalKey", "Set Normal Mode keybind", () => Key.None);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> ShoutKey = new("ShoutKey", "Set Shout Mode keybind", () => Key.None);

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<Key> BroadcastKey = new("BroadcastKey", "Set Broadcast Mode keybind", () => Key.None);

	private static ModConfiguration Config;

	public override void OnEngineInit() {
		Config = GetConfiguration();
		Config.Save(true);
		Harmony harmony = new Harmony("net.deltawolf.ResonitePTTKeybinds");
		harmony.PatchAll();
		Msg("VoiceMode keybinds patched!");
	}

	//TODO Config watcher, on changes beable to clear and set
	//Config.OnThisConfigurationChanged += handleConfigChanged;
	//globalActions.ToggleMute.clearbindings() then add new bindings
	[HarmonyPatch(typeof(KeyboardAndMouseBindingGenerator), "Bind")]
	class KeyBind_Patch {
		public static void Postfix(KeyboardAndMouseBindingGenerator __instance, InputGroup group) {
			try {
				if (group is GlobalActions globalActions) {
					if (Config.GetValue(remapKeys)) {
						globalActions.ToggleMute.ClearBindings();
						globalActions.ActivateTalk.ClearBindings();
						Debug("Clearing all PTT binds as remapKeys is enabled");
						try {
							globalActions.ToggleMute.AddBinding(InputNode.Key(Config.GetValue(customMuteKey)));
							Debug("Remapped Mute to: " + Config.GetValue(customMuteKey));
							globalActions.ActivateTalk.AddBinding(InputNode.Key(Config.GetValue(customPTTKey)).Gate(InputNode.Key(Key.Control).Invert(), true, false));
							Debug("Remapped PTT to: " + Config.GetValue(customPTTKey));
						} catch (Exception e) {
							Error(e); //likely invalid keycode
						}
					} else {
						if (Config.GetValue(disableMuteKey)) {
							globalActions.ToggleMute.ClearBindings();
							Debug("Cleared Mute Key");
						}
						if (Config.GetValue(disablePTTKeys)) {
							globalActions.ActivateTalk.ClearBindings();
							Debug("Cleared PTT Keys");
						}
					}
				}
			} catch (Exception e){
				Error(e);
			}
		}
	}

	[HarmonyPatch(typeof(VoiceModeSync), "OnCommonUpdate")]
	class VoiceMode_Patch {
		static void Postfix(VoiceModeSync __instance) {
			//Check if user is focused into a text field to type
			bool focus = __instance.LocalUser.HasActiveFocus() || (Engine.Current.WorldManager.FocusedWorld?.LocalUser.HasActiveFocus() ?? false);
			//ignore keybinds while in text fields
			if (focus) {
				return;
			}
			if (__instance.InputInterface.GetKeyDown(Config.GetValue(MuteKey))) {
				__instance.InputInterface.IsMuted = true;
				Debug("Mute Keybind pressed: " + Config.GetValue(MuteKey));
			}
			if (__instance.InputInterface.GetKeyDown(Config.GetValue(WhisperKey))) {
				if (VoiceMode.Whisper <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
					__instance.FocusedWorldVoiceMode.Value = VoiceMode.Whisper;
					__instance.InputInterface.IsMuted = false;
					Debug("Whisper Keybind pressed: " + Config.GetValue(WhisperKey));
				}
			}
			if (__instance.InputInterface.GetKeyDown(Config.GetValue(NormalKey))) {
				if (VoiceMode.Normal <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
					__instance.FocusedWorldVoiceMode.Value = VoiceMode.Normal;
					__instance.InputInterface.IsMuted = false;
					Debug("Normal Keybind pressed: " + Config.GetValue(NormalKey));
				}
			}
			if (__instance.InputInterface.GetKeyDown(Config.GetValue(ShoutKey))) {
				if (VoiceMode.Shout <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
					__instance.FocusedWorldVoiceMode.Value = VoiceMode.Shout;
					__instance.InputInterface.IsMuted = false;
					Debug("Shout Keybind pressed: " + Config.GetValue(ShoutKey));
				}
			}
			if (__instance.InputInterface.GetKeyDown(Config.GetValue(BroadcastKey))) {
				if (VoiceMode.Broadcast <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
					__instance.FocusedWorldVoiceMode.Value = VoiceMode.Broadcast;
					__instance.InputInterface.IsMuted = false;
					Debug("Broadcast Keybind pressed: " + Config.GetValue(BroadcastKey));
				}
			}
		}
	}

	[HarmonyPatch(typeof(UserspaceRadiantDash), "OnAttach")]
	class UserspaceRadiantDashOnAttach_Patch {
		static void Postfix(UserspaceRadiantDash __instance) {
			Slot overlayRoot = __instance.World.GetGloballyRegisteredComponent<OverlayManager>().OverlayRoot; // Root/Userspace/Overlay/
			Slot vmp = overlayRoot.AddSlot("VoiceModeProxy", false);
			VoiceModeSync vms = vmp.AttachComponent<VoiceModeSync>();
			ValueDriver<bool> vd = vmp.AttachComponent<ValueDriver<bool>>();
			vd.ValueSource.Target = vd.EnabledField;
			vd.DriveTarget.Target = vms.EnabledField;
			vmp.AttachComponent<Comment>().Text.Value = "Generated by ResonitePTTKeybinds by Delta";
		}
	}
}