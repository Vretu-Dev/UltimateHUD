﻿using System;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;

namespace UltimateHUD
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "UltimateHUD";
        public override string Author => "Vretu";
        public override string Prefix => "UltimateHUD";
        public override Version Version => new Version(5, 6, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);
        public static Plugin Instance { get; private set; }
        public HeaderSetting SettingsHeader { get; set; } = new HeaderSetting("Ultimate HUD");

        public override void OnEnabled()
        {
            Instance = this;
            SettingBase.Register(new[] { SettingsHeader });
            EventHandlers.RegisterEvents();
            ServerSettings.RegisterSettings();
            Hints.RegisterHints();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            SettingBase.Unregister(settings: new[] { SettingsHeader });
            EventHandlers.UnregisterEvents();
            ServerSettings.UnregisterSettings();
            Hints.UnregisterHints();
            base.OnDisabled();
        }
    }
}