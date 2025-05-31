using System;
using System.Linq;
using System.Collections.Generic;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using UnityEngine;

namespace UltimateHUD
{
    public static class Hints
    {
        private static readonly Dictionary<Player, Hint> clockHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> tpsHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> roundTimeHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> playerInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> serverInfoHints = new Dictionary<Player, Hint>();
        private static readonly Dictionary<Player, Hint> mapInfoHints = new Dictionary<Player, Hint>();
        public static Hint GetClockHint(Player player)
        {
            if (!clockHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (!Options.ShouldShow(Plugin.Instance.Config.ClockVisual, p)
                        || !Plugin.Instance.Config.EnableClock
                        || (p.SessionVariables.TryGetValue("ShowClock", out var showClock) && showClock is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        string timerColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        DateTime utc = DateTime.UtcNow.AddHours(Plugin.Instance.Config.TimeZone);

                        return Plugin.Instance.Config.Clock
                            .Replace("{color}", timerColor)
                            .Replace("{time}", utc.ToString("HH:mm"));
                    },
                    FontSize = 25,
                    YCoordinate = Plugin.Instance.Config.ClockYCordinate,
                    XCoordinate = Plugin.Instance.Config.ClockXCordinate,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
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
                        if (!Options.ShouldShow(Plugin.Instance.Config.TpsVisual, p)
                        || !Plugin.Instance.Config.EnableTps
                        || (p.SessionVariables.TryGetValue("ShowTps", out var showTps) && showTps is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        int tps = (int)Server.Tps;
                        int maxTps = (int)Server.MaxTps;
                        string tpsColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);

                        return Plugin.Instance.Config.Tps
                            .Replace("{color}", tpsColor)
                            .Replace("{tps}", tps.ToString())
                            .Replace("{maxTps}", maxTps.ToString());
                    },
                    FontSize = 25,
                    YCoordinate = Plugin.Instance.Config.TpsYCordinate,
                    XCoordinate = Plugin.Instance.Config.TpsXCordinate,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
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
                        if (!Options.ShouldShow(Plugin.Instance.Config.RoundTimeVisual, p)
                        || !Plugin.Instance.Config.EnableRoundTime
                        || (p.SessionVariables.TryGetValue("ShowRoundTime", out var showRoundTime) && showRoundTime is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        TimeSpan elapsed = Round.ElapsedTime;
                        string elapsedFormatted = elapsed.ToString(@"mm\:ss");
                        string elapsedColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);

                        return Plugin.Instance.Config.RoundTime
                            .Replace("{color}", elapsedColor)
                            .Replace("{round_time}", elapsedFormatted);
                    },
                    FontSize = 25,
                    YCoordinate = Plugin.Instance.Config.RoundTimeYCordinate,
                    XCoordinate = Plugin.Instance.Config.RoundTimeXCordinate,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
                };
                roundTimeHints[player] = hint;
            }
            return hint;
        }
        public static Hint GetPlayerInfoHint(Player player)
        {
            if (!playerInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.Role is SpectatorRole
                        || (p.SessionVariables.TryGetValue("ShowPlayerHUD", out var showPlayerHUD) && showPlayerHUD is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        string roleColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        string nickname = p.Nickname;
                        if (nickname.Length > 20)
                            nickname = nickname.Substring(0, 20) + "...";
                        uint id = (uint)p.Id;
                        string role = Plugin.Instance.Translation.GetRoleDisplayName(p);
                        string coloredRole = $"<color={roleColor}>{role}</color>";
                        int kills = EventHandlers.GetKills(p);

                        return Plugin.Instance.Config.PlayerHud
                            .Replace("{nickname}", nickname)
                            .Replace("{id}", id.ToString())
                            .Replace("{role}", coloredRole)
                            .Replace("{kills}", kills.ToString());
                    },
                    FontSize = 33,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
                };
                playerInfoHints[player] = hint;
            }
            return hint;
        }
        public static Hint GetSpectatorPlayerInfoHint(Player player)
        {
            if (!spectatorPlayerInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (!(p.Role is SpectatorRole spectatorRole)
                        || (p.SessionVariables.TryGetValue("ShowSpectatorHUD", out var showSpectatorHUD) && showSpectatorHUD is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        Player observed = spectatorRole.SpectatedPlayer;

                        if (observed == null)
                            return string.Empty;

                        string observedRoleColor = "#" + ColorUtility.ToHtmlStringRGB(observed.Role.Color);
                        string observedNickname = observed.Nickname;

                        if (observedNickname.Length > 16)
                            observedNickname = observedNickname.Substring(0, 16) + "...";

                        uint observedId = (uint)observed.Id;
                        string observedRole = Plugin.Instance.Translation.GetRoleDisplayName(observed);
                        string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                        int observedKills = EventHandlers.GetKills(observed);

                        return Plugin.Instance.Config.SpectatorHud
                            .Replace("{nickname}", observedNickname)
                            .Replace("{id}", observedId.ToString())
                            .Replace("{role}", coloredObservedRole)
                            .Replace("{kills}", observedKills.ToString());
                    },
                    FontSize = 33,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
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

                        if (!(p.Role is SpectatorRole)
                        || (p.SessionVariables.TryGetValue("ShowSpectatorHUD", out var showSpectatorHUD) && showSpectatorHUD is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                        int maxPlayers = Server.MaxPlayerCount;
                        int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);

                        return Plugin.Instance.Config.SpectatorServerInfo
                            .Replace("{players}", totalPlayers.ToString())
                            .Replace("{maxPlayers}", maxPlayers.ToString())
                            .Replace("{spectators}", spectators.ToString());
                    },
                    FontSize = 27,
                    YCoordinate = Plugin.Instance.Config.ServerInfoYCordinate,
                    XCoordinate = Plugin.Instance.Config.ServerInfoXCordinate,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
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

                        if (!(p.Role is SpectatorRole)
                        || (p.SessionVariables.TryGetValue("ShowSpectatorHUD", out var showSpectatorHUD) && showSpectatorHUD is bool enabled && !enabled)
                        || (p.SessionVariables.TryGetValue("ShowHUD", out var showHUD) && showHUD is bool display && !display))
                            return string.Empty;

                        int engaged = Generator.List.Count(g => g.IsEngaged);
                        int maxGenerators = 3;
                        bool isArmed = Warhead.Status == WarheadStatus.Armed;
                        string warheadStatus = isArmed ? "Armed" : "Not Armed";
                        string warheadColor = isArmed ? "red" : "green";

                        return Plugin.Instance.Config.SpectatorMapInfo
                            .Replace("{engaged}", engaged.ToString())
                            .Replace("{maxGenerators}", maxGenerators.ToString())
                            .Replace("{warheadColor}", warheadColor)
                            .Replace("{warheadStatus}", warheadStatus);
                    },
                    FontSize = 27,
                    YCoordinate = Plugin.Instance.Config.MapInfoYCordinate,
                    XCoordinate = Plugin.Instance.Config.MapInfoXCordinate,
                    SyncSpeed = Plugin.Instance.Config.HintSyncSpeed
                };
                mapInfoHints[player] = hint;
            }
            return hint;
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
                RemoveHints(player);
        }

    }
}
