using System;
using System.Linq;
using System.Collections.Generic;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using PlayerRoles;
using System.Text;
using System.Text.RegularExpressions;
using LabApi.Features.Wrappers;
using PlayerRoles.Spectating;

namespace UltimateHUD
{
    public static class Hints
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translations Translation => Plugin.Instance.Translation;

        private static readonly Dictionary<Player, Hint> clockHints = new();
        private static readonly Dictionary<Player, Hint> tpsHints = new();
        private static readonly Dictionary<Player, Hint> roundTimeHints = new();
        private static readonly Dictionary<Player, Hint> playerInfoHints = new();
        private static readonly Dictionary<Player, Hint> spectatingPlayerHints = new();
        private static readonly Dictionary<Player, Hint> ammoHints = new();
        private static readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new();
        private static readonly Dictionary<Player, Hint> serverInfoHints = new();
        private static readonly Dictionary<Player, Hint> mapInfoHints = new();


        // Hints for Everyone
        public static Hint GetClockHint(Player player)
        {
            if (!clockHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (!Options.ShouldShow(Config.ClockVisual, p))
                            return string.Empty;

                        string timerColor = Options.GetRoleColor(p);
                        DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);

                        return Config.Clock
                            .Replace("{color}", timerColor)
                            .Replace("{time}", utc.ToString("HH:mm"));
                    },

                    FontSize = Config.ClockFontSize,
                    YCoordinate = Config.ClockYCordinate,
                    XCoordinate = Config.ClockXCordinate,
                    SyncSpeed = HintSyncSpeed.Slowest

                };

                clockHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetTpsHint(Player player)
        {
            if (!tpsHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (!Options.ShouldShow(Config.TpsVisual, p))
                            return string.Empty;

                        int tps = (int)Server.Tps;
                        int maxTps = (int)Server.MaxTps;
                        string tpsColor = Options.GetRoleColor(p);

                        return Config.Tps
                            .Replace("{color}", tpsColor)
                            .Replace("{tps}", tps.ToString())
                            .Replace("{maxTps}", maxTps.ToString());
                    },

                    FontSize = Config.TpsFontSize,
                    YCoordinate = Config.TpsYCordinate,
                    XCoordinate = Config.TpsXCordinate,
                    SyncSpeed = HintSyncSpeed.Slowest
                };

                tpsHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetRoundTimeHint(Player player)
        {
            if (!roundTimeHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (!Options.ShouldShow(Config.RoundTimeVisual, p))
                            return string.Empty;

                        TimeSpan elapsed = Round.Duration;
                        string elapsedFormatted = elapsed.ToString(@"mm\:ss");
                        string elapsedColor = Options.GetRoleColor(p);

                        return Config.RoundTime
                            .Replace("{color}", elapsedColor)
                            .Replace("{round_time}", elapsedFormatted);
                    },

                    FontSize = Config.RoundTimeFontSize,
                    YCoordinate = Config.RoundTimeYCordinate,
                    XCoordinate = Config.RoundTimeXCordinate,
                    SyncSpeed = HintSyncSpeed.Normal

                };

                roundTimeHints[player] = hint;
            }

            return hint;
        }

        // Hints for Alive Players
        public static Hint GetPlayerInfoHint(Player player)
        {
            if (!playerInfoHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is SpectatorRole)
                    return null;

                string roleColor = Options.GetRoleColor(player);
                string nickname = player.Nickname;
                string displayname = player.DisplayName;

                displayname = Regex.Replace(displayname, "<color=#855439>\\*</color>$", "");

                if (nickname.Length > 20)
                    nickname = nickname.Substring(0, 20) + "...";

                if (displayname.Length > 20)
                    displayname = displayname.Substring(0, 20) + "...";

                uint id = (uint)player.PlayerId;
                string role = Plugin.Instance.Translation.GetRoleDisplayName(player);
                string coloredRole = $"<color={roleColor}>{role}</color>";
                int kills = EventHandlers.GetKills(player);

                string infoText = Config.PlayerHud
                        .Replace("{nickname}", nickname)
                        .Replace("{displayname}", displayname)
                        .Replace("{id}", id.ToString())
                        .Replace("{role}", coloredRole)
                        .Replace("{kills}", kills.ToString());

                hint = new Hint
                {
                    Text = infoText,
                    FontSize = Config.PlayerHudFontSize,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center
                };

                playerInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetSpectatingPlayer(Player player)
        {
            if (!spectatingPlayerHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is SpectatorRole || Config.HiddenForRoles.Contains(player.Role))
                    return null;

                var spectators = player.CurrentSpectators
                    .Where(s => s.Role != RoleTypeId.Overwatch)
                    .ToList();

                if (spectators.Count == 0)
                    return null;

                var sb = new StringBuilder();

                string color = Options.GetRoleColor(player);

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

                hint = new Hint
                {
                    Text = sb.ToString(),
                    FontSize = Config.SpectatorListFontSize,
                    YCoordinate = Config.SpectatorListYCordinate,
                    Alignment = HintAlignment.Right
                };

                spectatingPlayerHints[player] = hint;
            }

            return hint;
        }
        public static Hint GetAmmoHint(Player player)
        {
            if (!ammoHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is SpectatorRole || player.CurrentItem is not FirearmItem firearm)
                    return null;

                string color = Options.GetRoleColor(player);
                string weapon = Translation.GetWeaponDisplayName(firearm);

                string weaponName = Config.WeaponName
                    .Replace("{color}", color)
                    .Replace("{weapon}", weapon);

                string ammoCounter = Config.AmmoCounter
                    .Replace("{color}", color)
                    .Replace("{current}", firearm.StoredAmmo.ToString())
                    .Replace("{max}", firearm.MaxAmmo.ToString());

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(weaponName);
                sb.AppendLine(ammoCounter);

                hint = new Hint
                {
                    Text = sb.ToString(),
                    FontSize = Config.AmmoCounterFontSize,
                    YCoordinate = Config.AmmoCounterYCordinate
                };

                ammoHints[player] = hint;
            }

            return hint;
        }

        // Hints for Spectators
        public static Hint GetSpectatorPlayerInfoHint(Player player)
        {
            if (!spectatorPlayerInfoHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is not SpectatorRole spectatorRole)
                    return null;

                Player observed = player.CurrentlySpectating;

                if (observed == null)
                    return null;

                string observedRoleColor = Options.GetRoleColor(observed);
                string observedNickname = observed.Nickname;
                string observedDisplayname = observed.DisplayName;

                observedDisplayname = Regex.Replace(observedDisplayname, "<color=#855439>\\*</color>$", "");

                if (observedNickname.Length > 16)
                    observedNickname = observedNickname.Substring(0, 16) + "...";

                if (observedDisplayname.Length > 16)
                    observedDisplayname = observedDisplayname.Substring(0, 16) + "...";

                uint observedId = (uint)observed.PlayerId;
                string observedRole = Translation.GetRoleDisplayName(observed);
                string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                int observedKills = EventHandlers.GetKills(observed);

                if (Config.HideSkeletonNickname && observed.Role == RoleTypeId.Scp3114)
                    observedNickname = observedRole;

                string infoText = Config.SpectatorHud
                    .Replace("{nickname}", observedNickname)
                    .Replace("{displayname}", observedDisplayname)
                    .Replace("{id}", observedId.ToString())
                    .Replace("{role}", coloredObservedRole)
                    .Replace("{kills}", observedKills.ToString());

                hint = new Hint
                {
                    Text = infoText,
                    FontSize = Config.SpectatorHudFontSize,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center
                };

                spectatorPlayerInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetServerInfoHint(Player player)
        {
            if (!serverInfoHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is not SpectatorRole)
                    return null;

                int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                int maxPlayers = Server.MaxPlayers;
                int spectators = Player.List.Count(pl => pl.RoleBase is SpectatorRole && !pl.IsHost);

                string serverInfo = Config.SpectatorServerInfo
                    .Replace("{players}", totalPlayers.ToString())
                    .Replace("{maxPlayers}", maxPlayers.ToString())
                    .Replace("{spectators}", spectators.ToString());

                hint = new Hint
                {
                    Text = serverInfo,
                    FontSize = Config.ServerInfoFontSize,
                    YCoordinate = Config.ServerInfoYCordinate,
                    XCoordinate = Config.ServerInfoXCordinate
                };

                serverInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetMapInfoHint(Player player)
        {
            if (!mapInfoHints.TryGetValue(player, out var hint))
            {
                if (player.RoleBase is not SpectatorRole)
                    return null;

                int engaged = Generator.List.Count(g => g.Engaged);
                int maxGenerators = 3;

                WarheadStatus currentStatus = Options.GetCurrentWarheadStatus();
                string warheadStatus = Translation.GetWarheadStatusName(currentStatus);
                string warheadColor = Translation.GetWarheadStatusColor(currentStatus);

                string mapInfo = Config.SpectatorMapInfo
                    .Replace("{engaged}", engaged.ToString())
                    .Replace("{maxGenerators}", maxGenerators.ToString())
                    .Replace("{warheadColor}", warheadColor)
                    .Replace("{warheadStatus}", warheadStatus);

                hint = new Hint
                {
                    Text = mapInfo,
                    FontSize = Config.MapInfoFontSize,
                    YCoordinate = Config.MapInfoYCordinate,
                    XCoordinate = Config.MapInfoXCordinate
                };

                mapInfoHints[player] = hint;
            }

            return hint;
        }

        public static void AddHints(Player player)
        {
            AddClockHint(player);
            AddTpsHint(player);
            AddRoundTimeHint(player);
            AddPlayerInfoHint(player);
            AddSpectatingPlayerHint(player);
            AddAmmoHint(player);
            AddSpectatorPlayerInfoHint(player);
            AddServerInfoHint(player);
            AddMapInfoHint(player);
        }

        public static void RemoveHints(Player player)
        {
            RemoveClockHint(player);
            RemoveTpsHint(player);
            RemoveRoundTimeHint(player);
            RemovePlayerInfoHint(player);
            RemoveSpectatingPlayerHint(player);
            RemoveAmmoHint(player);
            RemoveSpectatorPlayerInfoHint(player);
            RemoveServerInfoHint(player);
            RemoveMapInfoHint(player);
        }

        public static void RemoveAllHints()
        {
            foreach (var player in Player.List.ToList())
                RemoveHints(player);
        }

        // Hints for Everyone
        public static void RemoveClockHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);
            if (clockHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                clockHints.Remove(player);
            }
        }

        public static void AddClockHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableClock)
                pd.AddHint(GetClockHint(player));
        }

        public static void RemoveTpsHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (tpsHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                tpsHints.Remove(player);
            }
        }

        public static void AddTpsHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableTps)
                pd.AddHint(GetTpsHint(player));
        }

        public static void RemoveRoundTimeHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (roundTimeHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                roundTimeHints.Remove(player);
            }
        }

        public static void AddRoundTimeHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableRoundTime)
                pd.AddHint(GetRoundTimeHint(player));
        }
        // Player Hints
        public static void RemovePlayerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (playerInfoHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                playerInfoHints.Remove(player);
            }
        }

        public static void AddPlayerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnablePlayerHud && player.RoleBase is not SpectatorRole)
                pd.AddHint(GetPlayerInfoHint(player));
        }

        public static void RemoveSpectatingPlayerHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (spectatingPlayerHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                spectatingPlayerHints.Remove(player);
            }
        }

        public static void AddSpectatingPlayerHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableSpectatorList && player.RoleBase is not SpectatorRole)
                pd.AddHint(GetSpectatingPlayer(player));
        }
        public static void RemoveAmmoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (ammoHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                ammoHints.Remove(player);
            }
        }

        public static void AddAmmoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableAmmoCounter && player.RoleBase is not SpectatorRole)
                pd.AddHint(GetAmmoHint(player));
        }
        // Spectators
        public static void RemoveSpectatorPlayerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (spectatorPlayerInfoHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                spectatorPlayerInfoHints.Remove(player);
            }
        }

        public static void AddSpectatorPlayerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableSpectatorHud && player.RoleBase is SpectatorRole)
                pd.AddHint(GetSpectatorPlayerInfoHint(player));
        }

        public static void RemoveServerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (serverInfoHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                serverInfoHints.Remove(player);
            }
        }

        public static void AddServerInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableSpectatorServerInfo && player.RoleBase is SpectatorRole)
                pd.AddHint(GetServerInfoHint(player));
        }

        public static void RemoveMapInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (mapInfoHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                mapInfoHints.Remove(player);
            }
        }

        public static void AddMapInfoHint(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableSpectatorMapInfo && player.RoleBase is SpectatorRole)
                pd.AddHint(GetMapInfoHint(player));
        }
    }
}