using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.Spectating;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using RueI.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UltimateHUD
{
    public static class Hints
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translations Translation => Plugin.Instance.Translation;

        private static readonly Tag ClockTag = new("hud_clock");
        private static readonly Tag TpsTag = new("hud_tps");
        private static readonly Tag RoundTimeTag = new("hud_roundtime");
        private static readonly Tag PlayerInfoTag = new("hud_playerinfo");
        private static readonly Tag AmmoTag = new("hud_ammo");
        private static readonly Tag SpectatorsListTag = new("hud_spectatorslist");
        private static readonly Tag SpectatorPlayerInfoTag = new("hud_spec_playerinfo");
        private static readonly Tag SpectatorServerInfoTag = new("hud_spec_serverinfo");
        private static readonly Tag SpectatorMapInfoTag = new("hud_spec_mapinfo");

        public static void RefreshAll(Player player)
        {
            RefreshClock(player);
            RefreshTps(player);
            RefreshRoundTime(player);
            RefreshPlayerInfo(player);
            RefreshAmmo(player);
            RefreshSpectatorList(player);
            RefreshSpectatorPlayerInfo(player);
            RefreshSpectatorServerInfo(player);
            RefreshSpectatorMapInfo(player);
        }

        public static void RemoveAll(Player player)
        {
            RueDisplay display = RueDisplay.Get(player);
            display.Remove(ClockTag);
            display.Remove(TpsTag);
            display.Remove(RoundTimeTag);
            display.Remove(PlayerInfoTag);
            display.Remove(AmmoTag);
            display.Remove(SpectatorsListTag);
            display.Remove(SpectatorPlayerInfoTag);
            display.Remove(SpectatorServerInfoTag);
            display.Remove(SpectatorMapInfoTag);
        }

        // =============== AUTO-REFRESH ===============

        public static void RefreshClock(Player player)
        {
            if (!Config.EnableClock) { RueDisplay.Get(player).Remove(ClockTag); return; }

            var element = new DynamicElement(
                position: Config.ClockYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Options.ShouldShow(Config.ClockVisual, p))
                        return string.Empty;

                    DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);
                    string color = Options.GetRoleColor(p);

                    return Config.Clock
                        .Replace("{color}", color)
                        .Replace("{time}", utc.ToString("HH:mm"));
                })
            {
                ZIndex = 2,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = false
            };

            RueDisplay.Get(player).Show(ClockTag, element);
        }

        public static void RefreshTps(Player player)
        {
            if (!Config.EnableTps) { RueDisplay.Get(player).Remove(TpsTag); return; }

            var element = new DynamicElement(
                position: Config.TpsYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p == null ||
                        !Options.ShouldShow(Config.TpsVisual, p))
                        return string.Empty;

                    int tps = (int)Server.Tps;
                    int maxTps = (int)Server.MaxTps;
                    string color = Options.GetRoleColor(p);

                    return Config.Tps
                        .Replace("{color}", color)
                        .Replace("{tps}", tps.ToString())
                        .Replace("{maxTps}", maxTps.ToString());
                })
            {
                ZIndex = 2,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = false
            };

            RueDisplay.Get(player).Show(TpsTag, element);
        }

        public static void RefreshRoundTime(Player player)
        {
            if (!Config.EnableRoundTime) { RueDisplay.Get(player).Remove(RoundTimeTag); return; }

            var element = new DynamicElement(
                position: Config.RoundTimeYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Options.ShouldShow(Config.RoundTimeVisual, p))
                        return string.Empty;

                    TimeSpan elapsed = Round.Duration;
                    string formatted = elapsed.ToString(@"mm\:ss");
                    string color = Options.GetRoleColor(p);

                    return Config.RoundTime
                        .Replace("{color}", color)
                        .Replace("{round_time}", formatted);
                })
            {
                ZIndex = 2,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = false
            };

            RueDisplay.Get(player).Show(RoundTimeTag, element);
        }

        // =============== EVENTBASE-REFRESH  ===============

        public static void RefreshPlayerInfo(Player player)
        {
            if (!Config.EnablePlayerHud) { RueDisplay.Get(player).Remove(PlayerInfoTag); return; }

            if (player == null ||
                player.RoleBase is SpectatorRole)
            {
                RueDisplay.Get(player).Remove(PlayerInfoTag);
                return;
            }

            string text = BuildPlayerInfo(player);

            var element = new BasicElement(Config.PlayerHudYCordinate, text)
            {
                ZIndex = 5,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(PlayerInfoTag, element);
        }

        public static void RefreshAmmo(Player player)
        {
            if (!Config.EnableAmmoCounter) { RueDisplay.Get(player).Remove(AmmoTag); return; }

            if (player == null ||
                player.RoleBase is SpectatorRole ||
                player.CurrentItem is not FirearmItem firearm)
            {
                RueDisplay.Get(player).Remove(AmmoTag);
                return;
            }

            string color = Options.GetRoleColor(player);
            string weapon = HintBuilding.Sanitize(Translation.GetWeaponDisplayName(firearm));

            string weaponLine = Config.WeaponName
                .Replace("{color}", color)
                .Replace("{weapon}", weapon);

            string ammoLine = Config.AmmoCounter
                .Replace("{color}", color)
                .Replace("{current}", firearm.StoredAmmo.ToString())
                .Replace("{max}", firearm.MaxAmmo.ToString());

            string final = weaponLine + "\n" + ammoLine;

            var element = new BasicElement(Config.AmmoCounterYCordinate, final)
            {
                ZIndex = 6,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(AmmoTag, element);
        }

        public static void RefreshSpectatorList(Player player)
        {
            if (!Config.EnableSpectatorList) { RueDisplay.Get(player).Remove(SpectatorsListTag); return; }

            if (player == null ||
                player.RoleBase is SpectatorRole ||
                Config.HiddenForRoles.Contains(player.Role))
            {
                RueDisplay.Get(player).Remove(SpectatorsListTag);
                return;
            }

            var spectators = player.CurrentSpectators
                .Where(s => s.Role != PlayerRoles.RoleTypeId.Overwatch)
                .ToList();

            if (spectators.Count == 0)
            {
                RueDisplay.Get(player).Remove(SpectatorsListTag);
                return;
            }

            string color = Options.GetRoleColor(player);
            var sb = new StringBuilder();
            sb.AppendLine(
                Config.SpectatorListHeader
                    .Replace("{count}", spectators.Count.ToString())
                    .Replace("{color}", color)
            );

            foreach (var spec in spectators)
            {
                string nick = TruncAndSanitize(spec.Nickname, 25);
                sb.AppendLine(
                    Config.SpectatorListPlayers
                        .Replace("{nickname}", nick)
                        .Replace("{color}", color)
                );
            }

            string text = sb.ToString().TrimEnd();

            var element = new BasicElement(Config.SpectatorListYCordinate, text)
            {
                ZIndex = 4,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(SpectatorsListTag, element);
        }

        public static void RefreshSpectatorPlayerInfo(Player player)
        {
            if (!Config.EnableSpectatorHud) { RueDisplay.Get(player).Remove(SpectatorPlayerInfoTag); return; }

            if (player?.RoleBase is not SpectatorRole)
            {
                RueDisplay.Get(player).Remove(SpectatorPlayerInfoTag);
                return;
            }

            Player observed = player.CurrentlySpectating;

            if (observed == null)
            {
                RueDisplay.Get(player).Remove(SpectatorPlayerInfoTag);
                return;
            }

            string text = BuildSpectatorObservedInfo(observed);

            var element = new BasicElement(Config.SpectatorHudYCordinate, text)
            {
                ZIndex = 6,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(SpectatorPlayerInfoTag, element);
        }

        public static void RefreshSpectatorServerInfo(Player player)
        {
            if (!Config.EnableSpectatorServerInfo) { RueDisplay.Get(player).Remove(SpectatorServerInfoTag); return; }

            if (player?.RoleBase is not SpectatorRole)
            {
                RueDisplay.Get(player).Remove(SpectatorServerInfoTag);
                return;
            }

            int totalPlayers = Player.ReadyList.Count(pl => !pl.IsHost);
            int maxPlayers = Server.MaxPlayers;
            int spectators = Player.ReadyList.Count(pl => pl.RoleBase is SpectatorRole && !pl.IsHost);

            string text = Config.SpectatorServerInfo
                .Replace("{players}", totalPlayers.ToString())
                .Replace("{maxPlayers}", maxPlayers.ToString())
                .Replace("{spectators}", spectators.ToString());

            var element = new BasicElement(Config.ServerInfoYCordinate, text)
            {
                ZIndex = 4,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(SpectatorServerInfoTag, element);
        }

        public static void RefreshSpectatorMapInfo(Player player)
        {
            if (!Config.EnableSpectatorMapInfo) { RueDisplay.Get(player).Remove(SpectatorMapInfoTag); return; }

            if (player?.RoleBase is not SpectatorRole)
            {
                RueDisplay.Get(player).Remove(SpectatorMapInfoTag);
                return;
            }

            int engaged = Generator.List.Count(g => g.Engaged);
            const int maxGenerators = 3;

            WarheadStatus currentStatus = Options.GetCurrentWarheadStatus();
            string warheadStatus = Translation.GetWarheadStatusName(currentStatus);
            string warheadColor = Translation.GetWarheadStatusColor(currentStatus);

            string text = Config.SpectatorMapInfo
                .Replace("{engaged}", engaged.ToString())
                .Replace("{maxGenerators}", maxGenerators.ToString())
                .Replace("{warheadColor}", warheadColor)
                .Replace("{warheadStatus}", warheadStatus);

            var element = new BasicElement(Config.MapInfoYCordinate, text)
            {
                ZIndex = 4,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(SpectatorMapInfoTag, element);
        }

        // =============== BUILDERS ===============

        private static string BuildPlayerInfo(Player p)
        {
            string roleColor = Options.GetRoleColor(p);
            string nickname = TruncAndSanitize(p.Nickname, 20);
            string displayName = TruncAndSanitize(RemoveStarSuffix(p.DisplayName), 20);

            uint id = (uint)p.PlayerId;
            string roleDisplay = Translation.GetRoleDisplayName(p);
            string coloredRole = $"<color={roleColor}>{HintBuilding.Sanitize(roleDisplay)}</color>";
            int kills = EventHandlers.GetKills(p);

            return Config.PlayerHud
                .Replace("{nickname}", nickname)
                .Replace("{displayname}", displayName)
                .Replace("{id}", id.ToString())
                .Replace("{role}", coloredRole)
                .Replace("{kills}", kills.ToString());
        }

        private static string BuildSpectatorObservedInfo(Player observed)
        {
            string roleColor = Options.GetRoleColor(observed);
            string nickname = TruncAndSanitize(observed.Nickname, 16);
            string displayName = TruncAndSanitize(RemoveStarSuffix(observed.DisplayName), 16);
            uint id = (uint)observed.PlayerId;

            if (Config.HideSkeletonNickname && observed.Role == RoleTypeId.Scp3114)
                nickname = Translation.GetRoleDisplayName(observed);

            string roleDisplay = Translation.GetRoleDisplayName(observed);
            string coloredRole = $"<color={roleColor}>{HintBuilding.Sanitize(roleDisplay)}</color>";
            int kills = EventHandlers.GetKills(observed);

            return Config.SpectatorHud
                .Replace("{nickname}", nickname)
                .Replace("{displayname}", displayName)
                .Replace("{id}", id.ToString())
                .Replace("{role}", coloredRole)
                .Replace("{kills}", kills.ToString());
        }

        // =============== HELPERS ===============

        private static string RemoveStarSuffix(string display) => Regex.Replace(display, "<color=#855439>\\*</color>$", "");

        private static string TruncAndSanitize(string str, int max)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length > max)
                str = str.Substring(0, max) + "...";

            return HintBuilding.Sanitize(str);
        }
    }
}