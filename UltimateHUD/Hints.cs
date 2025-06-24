using System;
using System.Linq;
using System.Collections.Generic;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles;
using System.Text;
using Exiled.API.Features.Items;
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
        private static readonly Dictionary<Player, Hint> ammoHints = new Dictionary<Player, Hint>();
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

                        if (!Options.ShouldShow(Config.ClockVisual, p) || !ServerSettings.ShouldShowClock(p) || !ServerSettings.ShouldShowHUD(p))
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

                        if (!Options.ShouldShow(Config.TpsVisual, p) || !ServerSettings.ShouldShowTps(p) || !ServerSettings.ShouldShowHUD(p))
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

                        if (!Options.ShouldShow(Config.RoundTimeVisual, p) || !ServerSettings.ShouldShowRoundTime(p) || !ServerSettings.ShouldShowHUD(p))
                            return string.Empty;

                        TimeSpan elapsed = Round.ElapsedTime;
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

                        if (p.Role is SpectatorRole || !ServerSettings.ShouldShowPlayerHUD(p) || !ServerSettings.ShouldShowHUD(p))
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

                        if (p.Role is SpectatorRole || Config.HiddenForRoles.Contains(p.Role.Type) || !ServerSettings.ShouldShowSpectatorList(p) || !ServerSettings.ShouldShowHUD(p))
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

                    FontSize = Config.SpectatorListFontSize,
                    YCoordinate = Config.SpectatorListYCordinate,
                    Alignment = HintAlignment.Right,
                    SyncSpeed = HintSyncSpeed.Normal

                };

                spectatingPlayerHints[player] = hint;
            }

            return hint;
        }

        public static Hint GetAmmoHint(Player player)
        {
            if (!ammoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.Role is SpectatorRole || p.CurrentItem is not Firearm firearm || !ServerSettings.ShouldShowAmmoCounter(p) || !ServerSettings.ShouldShowHUD(p))
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

                    FontSize = Config.AmmoCounterFontSize,
                    YCoordinate = Config.AmmoCounterYCordinate,
                    SyncSpeed = HintSyncSpeed.Fastest

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
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);

                        if (p.Role is not SpectatorRole spectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p))
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

                        if (p.Role is not SpectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p))
                            return string.Empty;

                        int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                        int maxPlayers = Server.MaxPlayerCount;
                        int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);

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

                        if (p.Role is not SpectatorRole || !ServerSettings.ShouldShowSpectatorHUD(p) || !ServerSettings.ShouldShowHUD(p))
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

                    FontSize = Config.MapInfoFontSize,
                    YCoordinate = Config.MapInfoYCordinate,
                    XCoordinate = Config.MapInfoXCordinate,
                    SyncSpeed = HintSyncSpeed.Slowest

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

            if (player.Role is SpectatorRole)
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

                if (Config.EnableAmmoCounter)
                    pd.AddHint(GetAmmoHint(player));
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

            if (ammoHints.TryGetValue(player, out hint))
            {
                pd.RemoveHint(hint);
                ammoHints.Remove(player);
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
