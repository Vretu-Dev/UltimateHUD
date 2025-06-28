using System;
using System.Linq;
using System.Collections.Generic;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using UnityEngine;
using LabApi.Features.Wrappers;
using PlayerRoles.Spectating;
using PlayerRoles;
using System.Text;
using System.Text.RegularExpressions;

namespace UltimateHUD
{
    public static class Hints
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translations Translation => Plugin.Instance.Translation;

        private static readonly Dictionary<Player, Hint> clockHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> tpsHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> roundTimeHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> playerInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> spectatingPlayerHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> serverInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> mapInfoHints = new Dictionary<Player, Hint>();

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
                    SyncSpeed = HintSyncSpeed.Slow
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
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.RoleBase is SpectatorRole)
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

                    FontSize = Config.PlayerHudFontSize,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = HintSyncSpeed.Slow
                };

                playerInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetSpectatingPlayer(Player player)
        {
            if (!spectatingPlayerHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.RoleBase is SpectatorRole || Config.HiddenForRoles.Contains(p.Role))
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

                    FontSize = Config.SpectatorListFontSize,
                    YCoordinate = Config.SpectatorListYCordinate,
                    Alignment = HintAlignment.Right,
                    SyncSpeed = HintSyncSpeed.Normal
                };

                spectatingPlayerHints[player] = hint;
            }

            return hint;
        }

        // Hints for Spectators
        public static Hint GetSpectatorPlayerInfoHint(Player player)
        {
            if (!spectatorPlayerInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.RoleBase is not SpectatorRole)
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

                    FontSize = Config.SpectatorHudFontSize,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = HintSyncSpeed.Normal
                };

                spectatorPlayerInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetServerInfoHint(Player player)
        {
            if (!serverInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.RoleBase is not SpectatorRole)
                            return string.Empty;

                        int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                        int maxPlayers = Server.MaxPlayers;
                        int spectators = Player.List.Count(pl => pl.Role == RoleTypeId.Spectator && !pl.IsHost);

                        return Config.SpectatorServerInfo
                            .Replace("{players}", totalPlayers.ToString())
                            .Replace("{maxPlayers}", maxPlayers.ToString())
                            .Replace("{spectators}", spectators.ToString());
                    },

                    FontSize = Config.ServerInfoFontSize,
                    YCoordinate = Config.ServerInfoYCordinate,
                    XCoordinate = Config.ServerInfoXCordinate,
                    SyncSpeed = HintSyncSpeed.Slowest
                };

                serverInfoHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetMapInfoHint(Player player)
        {
            if (!mapInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.RoleBase is not SpectatorRole)
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

                    FontSize = Config.MapInfoFontSize,
                    YCoordinate = Config.MapInfoYCordinate,
                    XCoordinate = Config.MapInfoXCordinate,
                    SyncSpeed = HintSyncSpeed.Normal
                };

                mapInfoHints[player] = hint;
            }

            return hint;
        }

        public static void AddHints(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (Config.EnableClock)
                pd.AddHint(GetClockHint(player));

            if (Config.EnableTps)
                pd.AddHint(GetTpsHint(player));

            if (Config.EnableRoundTime)
                pd.AddHint(GetRoundTimeHint(player));

            if (player.RoleBase is SpectatorRole)
            {
                if(Config.EnableSpectatorHud)
                    pd.AddHint(GetSpectatorPlayerInfoHint(player));

                if (Config.EnableSpectatorServerInfo)
                    pd.AddHint(GetServerInfoHint(player));

                if (Config.EnableSpectatorMapInfo)
                    pd.AddHint(GetMapInfoHint(player));
            }
            else
            {
                if (Config.EnablePlayerHud)
                    pd.AddHint(GetPlayerInfoHint(player));

                if (Config.EnableSpectatorList)
                    pd.AddHint(GetSpectatingPlayer(player));
            }
        }

        public static void RemoveHints(Player player)
        {
            PlayerDisplay pd = PlayerDisplay.Get(player);

            if (clockHints.TryGetValue(player, out var hint))
            {
                pd.RemoveHint(hint);
                clockHints.Remove(player);
            }
            if (tpsHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                tpsHints.Remove(player);
            }
            if (roundTimeHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                roundTimeHints.Remove(player);
            }
            if (playerInfoHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                playerInfoHints.Remove(player);
            }
            if (spectatingPlayerHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                spectatingPlayerHints.Remove(player);
            }
            if (spectatorPlayerInfoHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                spectatorPlayerInfoHints.Remove(player);
            }
            if (serverInfoHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                serverInfoHints.Remove(player);
            }
            if (mapInfoHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                mapInfoHints.Remove(player);
            }
        }

        public static void RemoveAllHints()
        {
            foreach (var player in Player.List.ToList())
                if (!player.IsHost)
                    RemoveHints(player);
        }

    }
}
