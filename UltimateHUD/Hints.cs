using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.Spectating;
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

                if (!Options.ShouldShow(Config.ClockVisual, p) || Config.EnableClock)
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

                if (!Options.ShouldShow(Config.TpsVisual, p) || Config.EnableTps)
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

                if (!Options.ShouldShow(Config.RoundTimeVisual, p) || Config.EnableRoundTime)
                    return string.Empty;

                TimeSpan elapsed = Round.Duration;
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

                if (p.RoleBase is SpectatorRole || Config.EnablePlayerHud)
                    return string.Empty;

                string roleColor = Options.GetRoleColor(p);
                string nickname = p.Nickname;
                string displayname = p.DisplayName;

                displayname = Regex.Replace(displayname, "<color=#855439>\\*</color>$", "");

                if (nickname.Length > 20)
                    nickname = nickname.Substring(0, 20) + "...";

                if (displayname.Length > 20)
                    displayname = displayname.Substring(0, 20) + "...";

                uint id = (uint)p.PlayerId;
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

        // Spectator Players List
        public static DynamicElement SpectatingPlayersElement = new(
            core =>
            {
                var p = Player.Get(core.Hub);

                if (p.RoleBase is SpectatorRole || Config.HiddenForRoles.Contains(p.Role) || Config.EnableSpectatorList)
                    return string.Empty;

                var spectators = p.CurrentSpectators
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

                if (p.RoleBase is not SpectatorRole || Config.EnableSpectatorHud)
                    return string.Empty;

                Player observed = p.CurrentlySpectating;

                if (observed == null)
                    return string.Empty;

                string observedRoleColor = Options.GetRoleColor(observed);
                string observedNickname = observed.Nickname;
                string observedDisplayname = observed.DisplayName;

                observedDisplayname = Regex.Replace(observedDisplayname, "<color=#855439>\\*</color>$", "");

                if (observedNickname.Length > 14)
                    observedNickname = observedNickname.Substring(0, 16) + "...";

                if (observedDisplayname.Length > 16)
                    observedDisplayname = observedDisplayname.Substring(0, 16) + "...";

                uint observedId = (uint)observed.PlayerId;
                string observedRole = Translation.GetRoleDisplayName(observed);
                string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                int observedKills = EventHandlers.GetKills(observed);

                if (Config.HideSkeletonNickname && observed.RoleBase.RoleTypeId == RoleTypeId.Scp3114)
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

                if (p.RoleBase is not SpectatorRole || Config.EnableSpectatorServerInfo)
                    return string.Empty;

                int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                int maxPlayers = Server.MaxPlayers;
                int spectators = Player.List.Count(pl => pl.Role == RoleTypeId.Spectator && !pl.IsHost);

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

                if (p.RoleBase is not SpectatorRole || Config.EnableSpectatorMapInfo)
                    return string.Empty;

                int engaged = Generator.List.Count(g => g.Engaged);
                int maxGenerators = 3;

                WarheadStatus currentStatus = Options.GetCurrentWarheadStatus();
                string warheadStatus = Translation.GetWarheadStatusName(currentStatus);
                string warheadColor = Translation.GetWarheadStatusColor(currentStatus);

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
        }
    }
}