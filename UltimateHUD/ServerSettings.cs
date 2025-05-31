using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;

namespace UltimateHUD
{
    public static class ServerSpecificSettings
    {
        public static TwoButtonsSetting HUD { get; private set; }
        public static TwoButtonsSetting Clock { get; private set; }
        public static TwoButtonsSetting Tps { get; private set; }
        public static TwoButtonsSetting RoundTime { get; private set; }
        public static TwoButtonsSetting PlayerHUD { get; private set; }
        public static TwoButtonsSetting SpectatorHUD { get; private set; }
        public static void RegisterSettings()
        {
            HUD = new TwoButtonsSetting(
                id: 5554,
                label: "Display HUD",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Disable or Enable all HUD",
                onChanged: (player, setting) =>
                {
                    var showHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowHUD"] = showHUD;
                });
            SettingBase.Register(new[] { HUD });

            if (Plugin.Instance.Config.EnableClock)
            {
                Clock = new TwoButtonsSetting(
                id: 5555,
                label: "Clock",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Should Clock be displayed",
                onChanged: (player, setting) =>
                {
                    var showClock = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowClock"] = showClock;
                });
                SettingBase.Register(new[] { Clock });
            }
            if (Plugin.Instance.Config.EnableTps)
            {
                Tps = new TwoButtonsSetting(
                id: 5556,
                label: "TPS",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Should TPS be displayed",
                onChanged: (player, setting) =>
                {
                    var showTps = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowTps"] = showTps;
                });
                SettingBase.Register(new[] { Tps });
            }
            if (Plugin.Instance.Config.EnableRoundTime)
            {
                RoundTime = new TwoButtonsSetting(
                id: 5557,
                label: "Round Time",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Should Round Time be displayed",
                onChanged: (player, setting) =>
                {
                    var showRoundTime = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowRoundTime"] = showRoundTime;
                });
                SettingBase.Register(new[] { RoundTime });
            }

            PlayerHUD = new TwoButtonsSetting(
                id: 5558,
                label: "Gameplay HUD",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Should Player HUD be displayed",
                onChanged: (player, setting) =>
                {
                    var showPlayerHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowPlayerHUD"] = showPlayerHUD;
                });
            SettingBase.Register(new[] { PlayerHUD });

            SpectatorHUD = new TwoButtonsSetting(
                id: 5559,
                label: "Spectator HUD",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: false,
                hintDescription: "Should Spectator HUD be displayed",
                onChanged: (player, setting) =>
                {
                    var showSpectatorHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowSpectatorHUD"] = showSpectatorHUD;
                });
            SettingBase.Register(new[] { SpectatorHUD });
        }
        public static void UnregisterSettings()
        {
            SettingBase.Unregister(settings: new[] { HUD });
            if (Clock != null)
                SettingBase.Unregister(settings: new[] { Clock });
            if (Tps != null)
                SettingBase.Unregister(settings: new[] { Tps });
            if (RoundTime != null)
                SettingBase.Unregister(settings: new[] { RoundTime });
            SettingBase.Unregister(settings: new[] { PlayerHUD });
            SettingBase.Unregister(settings: new[] { SpectatorHUD });
        }
    }
}