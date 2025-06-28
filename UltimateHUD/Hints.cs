using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using PlayerRoles;
using RueI;
using RueI.Displays;
using RueI.Elements;

namespace UltimateHUD
{
    public static class Hints
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translations Translation => Plugin.Instance.Translation;

        // Hints for Everyone
        // Clock Hint
        public static DynamicElement ClockElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (!Options.ShouldShow(Config.ClockVisual, p) || !ServerSettings.ShouldShowClock(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableClock)
                    return string.Empty;

                string timerColor = Options.GetRoleColor(p);
                DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);

                return Config.Clock
                    .Replace("{color}", timerColor)
                    .Replace("{time}", utc.ToString("HH:mm"));
            },
            Config.ClockYCordinate
        );

        public static AutoElement ClockAuto = new(Roles.All, ClockElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(2))
        };

        // TPS Hint
        public static DynamicElement TpsElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (!Options.ShouldShow(Config.TpsVisual, p) || !ServerSettings.ShouldShowTps(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableTps)
                    return string.Empty;

                int tps = (int)Server.Tps;
                int maxTps = (int)Server.MaxTps;
                string tpsColor = Options.GetRoleColor(p);

                return Config.Tps
                    .Replace("{color}", tpsColor)
                    .Replace("{tps}", tps.ToString())
                    .Replace("{maxTps}", maxTps.ToString());
            },
            Config.TpsYCordinate
        );

        public static AutoElement TpsAuto = new(Roles.All, TpsElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(2))
        };

        // Round Time Hint
        public static DynamicElement RoundTimeElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (!Options.ShouldShow(Config.RoundTimeVisual, p) || !ServerSettings.ShouldShowRoundTime(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableRoundTime)
                    return string.Empty;

                TimeSpan elapsed = Round.ElapsedTime;
                string elapsedFormatted = elapsed.ToString(@"mm\:ss");
                string elapsedColor = Options.GetRoleColor(p);

                return Config.RoundTime
                    .Replace("{color}", elapsedColor)
                    .Replace("{round_time}", elapsedFormatted);
            },
            Config.RoundTimeYCordinate
        );

        public static AutoElement RoundTimeAuto = new(Roles.All, RoundTimeElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(1))
        };

        // Hints for Alive Players
        // Player HUD
        public static DynamicElement PlayerInfoElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is SpectatorRole || !ServerSettings.ShouldShowPlayerHUD(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnablePlayerHud)
                    return string.Empty;

                string roleColor = Options.GetRoleColor(p);
                string nickname = p.Nickname;
                string displayname = p.DisplayNickname;

                displayname = Regex.Replace(displayname, "<color=#855439>\\*</color>$", "");

                if (nickname.Length > 20)
                    nickname = nickname.Substring(0, 20) + "...";

                if (displayname.Length > 20)
                    displayname = displayname.Substring(0, 20) + "...";

                uint id = (uint)p.Id;
                string role = Translation.GetRoleDisplayName(p);
                string coloredRole = $"<color={roleColor}>{role}</color>";
                int kills = EventHandlers.GetKills(p);

                return Config.PlayerHud
                    .Replace("{nickname}", nickname)
                    .Replace("{displayname}", displayname)
                    .Replace("{id}", id.ToString())
                    .Replace("{role}", coloredRole)
                    .Replace("{kills}", kills.ToString());
            },
            Config.PlayerHudYCordinate
        );

        public static AutoElement PlayerInfoAuto = new(Roles.Alive, PlayerInfoElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(1))
        };

        // Ammo Counter
        public static DynamicElement AmmoElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is SpectatorRole || p.CurrentItem is not Firearm firearm || !ServerSettings.ShouldShowAmmoCounter(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableAmmoCounter)
                    return string.Empty;

                string color = Options.GetRoleColor(p);
                string weapon = Translation.GetWeaponDisplayName(firearm);

                string weaponName = Config.WeaponName
                    .Replace("{color}", color)
                    .Replace("{weapon}", weapon);

                string ammoCounter = Config.AmmoCounter
                    .Replace("{color}", color)
                    .Replace("{current}", firearm.TotalAmmo.ToString())
                    .Replace("{max}", firearm.TotalMaxAmmo.ToString());

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(weaponName);
                sb.AppendLine(ammoCounter);

                return sb.ToString();
            },
            Config.AmmoCounterYCordinate
        );

        public static AutoElement AmmoAuto = new(Roles.Alive, AmmoElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(0.05))
        };

        // Spectator Players List
        public static DynamicElement SpectatingPlayersElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is SpectatorRole || Config.HiddenForRoles.Contains(p.Role.Type) || !ServerSettings.ShouldShowSpectatorList(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableSpectatorList)
                    return string.Empty;

                var spectators = p.CurrentSpectatingPlayers
                    .Where(s => s.Role != RoleTypeId.Overwatch)
                    .ToList();

                if (spectators.Count == 0)
                    return string.Empty;

                var sb = new StringBuilder();

                string color = Options.GetRoleColor(p);

                sb.AppendLine(
                    Config.SpectatorListHeader
                        .Replace("{count}", spectators.Count.ToString())
                        .Replace("{color}", color)
                );

                foreach (var spectator in spectators)
                {
                    sb.AppendLine(
                        Config.SpectatorListPlayers
                            .Replace("{nickname}", spectator.Nickname)
                            .Replace("{color}", color)
                    );
                }

                return sb.ToString();
            },
            Config.SpectatorListYCordinate
        );

        public static AutoElement SpectatingPlayersAuto = new(Roles.Alive, SpectatingPlayersElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(1))
        };

        // Hints for Spectators
        // Spectator HUD
        public static DynamicElement SpectatorPlayerInfoElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is not SpectatorRole spectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableSpectatorHud)
                    return string.Empty;

                Player observed = spectatorRole.SpectatedPlayer;

                if (observed == null)
                    return string.Empty;

                string observedRoleColor = Options.GetRoleColor(observed);
                string observedNickname = observed.Nickname;
                string observedDisplayname = observed.DisplayNickname;

                observedDisplayname = Regex.Replace(observedDisplayname, "<color=#855439>\\*</color>$", "");

                if (observedNickname.Length > 16)
                    observedNickname = observedNickname.Substring(0, 16) + "...";

                if (observedDisplayname.Length > 16)
                    observedDisplayname = observedDisplayname.Substring(0, 16) + "...";

                uint observedId = (uint)observed.Id;
                string observedRole = Translation.GetRoleDisplayName(observed);
                string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                int observedKills = EventHandlers.GetKills(observed);

                if (Config.HideSkeletonNickname && observed.Role.Type == RoleTypeId.Scp3114)
                            observedNickname = observedRole;

                return Config.SpectatorHud
                    .Replace("{nickname}", observedNickname)
                    .Replace("{displayname}", observedDisplayname)
                    .Replace("{id}", observedId.ToString())
                    .Replace("{role}", coloredObservedRole)
                    .Replace("{kills}", observedKills.ToString());
            },
            Config.SpectatorHudYCordinate
        );

        public static AutoElement SpectatorPlayerInfoAuto = new(Roles.Spectator, SpectatorPlayerInfoElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(1))
        };

        // Server Info
        public static DynamicElement ServerInfoElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is not SpectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p)|| Config.EnableSpectatorServerInfo)
                    return string.Empty;

                int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                int maxPlayers = Server.MaxPlayerCount;
                int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);

                return Config.SpectatorServerInfo
                    .Replace("{players}", totalPlayers.ToString())
                    .Replace("{maxPlayers}", maxPlayers.ToString())
                    .Replace("{spectators}", spectators.ToString());
            },
            Config.ServerInfoYCordinate
        );

        public static AutoElement ServerInfoAuto = new(Roles.Spectator, ServerInfoElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(2))
        };

        // Map Info
        public static DynamicElement MapInfoElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.Role is not SpectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p) || Config.EnableSpectatorMapInfo)
                    return string.Empty;

                int engaged = Generator.List.Count(g => g.IsEngaged);
                int maxGenerators = 3;
                string warheadStatus = Translation.GetWarheadStatusName(Warhead.Status);
                string warheadColor = Translation.GetWarheadStatusColor(Warhead.Status);

                return Config.SpectatorMapInfo
                    .Replace("{engaged}", engaged.ToString())
                    .Replace("{maxGenerators}", maxGenerators.ToString())
                    .Replace("{warheadColor}", warheadColor)
                    .Replace("{warheadStatus}", warheadStatus);
            },
            Config.MapInfoYCordinate
        );

        public static AutoElement MapInfoAuto = new(Roles.Spectator, MapInfoElement)
        {
            UpdateEvery = new AutoElement.PeriodicUpdate(TimeSpan.FromSeconds(2))
        };

        public static void RegisterHints()
        {
            RueIMain.EnsureInit();
        }

        public static void UnregisterHints()
        {
            ClockAuto.Disable();
            TpsAuto.Disable();
            RoundTimeAuto.Disable();
            PlayerInfoAuto.Disable();
            SpectatorPlayerInfoAuto.Disable();
            ServerInfoAuto.Disable();
            MapInfoAuto.Disable();
            SpectatingPlayersAuto.Disable();
            AmmoAuto.Disable();
        }
    }
}