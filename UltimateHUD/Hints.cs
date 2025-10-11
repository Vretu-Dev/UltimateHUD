using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using RueI.Utils;

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

        public static void RefreshAll(Player p)
        {
            var hub = p.ReferenceHub;
            RefreshClock(hub);
            RefreshTps(hub);
            RefreshRoundTime(hub);
            RefreshPlayerInfo(hub);
            RefreshAmmo(hub);
            RefreshSpectatorList(hub);
            RefreshSpectatorPlayerInfo(hub);
            RefreshSpectatorServerInfo(hub);
            RefreshSpectatorMapInfo(hub);
        }

        public static void RemoveAll(Player p)
        {
            var d = RueDisplay.Get(p.ReferenceHub);
            d.Remove(ClockTag);
            d.Remove(TpsTag);
            d.Remove(RoundTimeTag);
            d.Remove(PlayerInfoTag);
            d.Remove(AmmoTag);
            d.Remove(SpectatorsListTag);
            d.Remove(SpectatorPlayerInfoTag);
            d.Remove(SpectatorServerInfoTag);
            d.Remove(SpectatorMapInfoTag);
        }

        // =============== AUTO-REFRESH ===============

        public static void RefreshClock(ReferenceHub hub)
        {
            if (!Config.EnableClock) { RueDisplay.Get(hub).Remove(ClockTag); return; }

            var element = new DynamicElement(
                position: Config.ClockYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Options.ShouldShow(Config.ClockVisual, p) ||
                        !ServerSettings.ShouldShowClock(p) ||
                        !ServerSettings.ShouldShowHUD(p))
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

            RueDisplay.Get(hub).Show(ClockTag, element);
        }

        public static void RefreshTps(ReferenceHub hub)
        {
            if (!Config.EnableTps) { RueDisplay.Get(hub).Remove(TpsTag); return; }

            var element = new DynamicElement(
                position: Config.TpsYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p == null ||
                        !Options.ShouldShow(Config.TpsVisual, p) ||
                        !ServerSettings.ShouldShowTps(p) ||
                        !ServerSettings.ShouldShowHUD(p))
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

            RueDisplay.Get(hub).Show(TpsTag, element);
        }

        public static void RefreshRoundTime(ReferenceHub hub)
        {
            if (!Config.EnableRoundTime) { RueDisplay.Get(hub).Remove(RoundTimeTag); return; }

            var element = new DynamicElement(
                position: Config.RoundTimeYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Options.ShouldShow(Config.RoundTimeVisual, p) ||
                        !ServerSettings.ShouldShowRoundTime(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    TimeSpan elapsed = Round.ElapsedTime;
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

            RueDisplay.Get(hub).Show(RoundTimeTag, element);
        }

        // =============== EVENTBASE-REFRESH  ===============

        public static void RefreshPlayerInfo(ReferenceHub hub)
        {
            if (!Config.EnablePlayerHud) { RueDisplay.Get(hub).Remove(PlayerInfoTag); return; }

            var p = Player.Get(hub);

            if (p == null ||
                p.Role is SpectatorRole ||
                !ServerSettings.ShouldShowPlayerHUD(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(PlayerInfoTag);
                return;
            }

            string text = BuildPlayerInfo(p);

            var element = new BasicElement(Config.PlayerHudYCordinate, text)
            {
                ZIndex = 5,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(hub).Show(PlayerInfoTag, element);
        }

        public static void RefreshAmmo(ReferenceHub hub)
        {
            if (!Config.EnableAmmoCounter) { RueDisplay.Get(hub).Remove(AmmoTag); return; }

            var p = Player.Get(hub);

            if (p == null ||
                p.Role is SpectatorRole ||
                p.CurrentItem is not Firearm firearm ||
                !ServerSettings.ShouldShowAmmoCounter(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(AmmoTag);
                return;
            }

            string color = Options.GetRoleColor(p);
            string weapon = HintBuilding.Sanitize(Translation.GetWeaponDisplayName(firearm));

            string weaponLine = Config.WeaponName
                .Replace("{color}", color)
                .Replace("{weapon}", weapon);

            string ammoLine = Config.AmmoCounter
                .Replace("{color}", color)
                .Replace("{current}", firearm.TotalAmmo.ToString())
                .Replace("{max}", firearm.TotalMaxAmmo.ToString());

            string final = weaponLine + "\n" + ammoLine;

            var element = new BasicElement(Config.AmmoCounterYCordinate, final)
            {
                ZIndex = 6,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(hub).Show(AmmoTag, element);
        }

        public static void RefreshSpectatorList(ReferenceHub hub)
        {
            if (!Config.EnableSpectatorList) { RueDisplay.Get(hub).Remove(SpectatorsListTag); return; }

            var p = Player.Get(hub);

            if (p == null ||
                p.Role is SpectatorRole ||
                Config.HiddenForRoles.Contains(p.Role.Type) ||
                !ServerSettings.ShouldShowSpectatorList(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(SpectatorsListTag);
                return;
            }

            var spectators = p.CurrentSpectatingPlayers
                .Where(s => s.Role != PlayerRoles.RoleTypeId.Overwatch)
                .ToList();

            if (spectators.Count == 0)
            {
                RueDisplay.Get(hub).Remove(SpectatorsListTag);
                return;
            }

            string color = Options.GetRoleColor(p);
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

            RueDisplay.Get(hub).Show(SpectatorsListTag, element);
        }

        public static void RefreshSpectatorPlayerInfo(ReferenceHub hub)
        {
            if (!Config.EnableSpectatorHud) { RueDisplay.Get(hub).Remove(SpectatorPlayerInfoTag); return; }

            var p = Player.Get(hub);

            if (p?.Role is not SpectatorRole specRole ||
                !ServerSettings.ShouldShowSpectatorHUD(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(SpectatorPlayerInfoTag);
                return;
            }

            Player observed = specRole.SpectatedPlayer;

            if (observed == null)
            {
                RueDisplay.Get(hub).Remove(SpectatorPlayerInfoTag);
                return;
            }

            string text = BuildSpectatorObservedInfo(observed);

            var element = new BasicElement(Config.SpectatorHudYCordinate, text)
            {
                ZIndex = 6,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(hub).Show(SpectatorPlayerInfoTag, element);
        }

        public static void RefreshSpectatorServerInfo(ReferenceHub hub)
        {
            if (!Config.EnableSpectatorServerInfo) { RueDisplay.Get(hub).Remove(SpectatorServerInfoTag); return; }

            var p = Player.Get(hub);

            if (p?.Role is not SpectatorRole ||
                !ServerSettings.ShouldShowSpectatorHUD(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(SpectatorServerInfoTag);
                return;
            }

            int totalPlayers = Player.List.Count(pl => !pl.IsHost);
            int maxPlayers = Server.MaxPlayerCount;
            int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);

            string text = Config.SpectatorServerInfo
                .Replace("{players}", totalPlayers.ToString())
                .Replace("{maxPlayers}", maxPlayers.ToString())
                .Replace("{spectators}", spectators.ToString());

            var element = new BasicElement(Config.ServerInfoYCordinate, text)
            {
                ZIndex = 4,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(hub).Show(SpectatorServerInfoTag, element);
        }

        public static void RefreshSpectatorMapInfo(ReferenceHub hub)
        {
            if (!Config.EnableSpectatorMapInfo) { RueDisplay.Get(hub).Remove(SpectatorMapInfoTag); return; }

            var p = Player.Get(hub);

            if (p?.Role is not SpectatorRole ||
                !ServerSettings.ShouldShowSpectatorHUD(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(hub).Remove(SpectatorMapInfoTag);
                return;
            }

            int engaged = Generator.List.Count(g => g.IsEngaged);
            const int maxGenerators = 3;
            string warheadStatus = Translation.GetWarheadStatusName(Warhead.Status);
            string warheadColor = Translation.GetWarheadStatusColor(Warhead.Status);

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

            RueDisplay.Get(hub).Show(SpectatorMapInfoTag, element);
        }

        // =============== BUILDERS ===============

        private static string BuildPlayerInfo(Player p)
        {
            string roleColor = Options.GetRoleColor(p);
            string nickname = TruncAndSanitize(p.Nickname, 20);
            string displayName = TruncAndSanitize(RemoveStarSuffix(p.DisplayNickname), 20);

            uint id = (uint)p.Id;
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
            string displayName = TruncAndSanitize(RemoveStarSuffix(observed.DisplayNickname), 16);
            uint id = (uint)observed.Id;

            if (Config.HideSkeletonNickname && observed.Role.Type == RoleTypeId.Scp3114)
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