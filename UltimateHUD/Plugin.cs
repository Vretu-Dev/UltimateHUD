using System;
using System.Linq;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Enum;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using PlayerRoles;
using UnityEngine;

namespace UltimateHUD
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "UltimateHUD";
        public override string Author => "Vretu";
        public override string Prefix => "UltimateHud";
        public override Version Version => new Version(3, 0, 0);
        public static Plugin Instance { get; private set; }

        private readonly Dictionary<Player, Hint> clockHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> tpsHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> roundTimeHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> playerInfoHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> serverInfoHints = new Dictionary<Player, Hint>();
        private readonly Dictionary<Player, Hint> mapInfoHints = new Dictionary<Player, Hint>();

        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Left += OnLeftPlayer;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Left -= OnLeftPlayer;
            RemoveAllHints();
            base.OnDisabled();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            RemoveAllHints();
            playerKills.Clear();
        }

        private void OnLeftPlayer(LeftEventArgs ev)
        {
            RemoveHints(ev.Player);
            playerKills.Remove(ev.Player);
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            PlayerDisplay pd = PlayerDisplay.Get(ev.Player);

            RemoveHints(ev.Player);

            if (ev.NewRole == RoleTypeId.Spectator)
            {
                pd.AddHint(GetClockHint(ev.Player));
                pd.AddHint(GetTpsHint(ev.Player));
                pd.AddHint(GetRoundTimeHint(ev.Player));
                pd.AddHint(GetSpectatorPlayerInfoHint(ev.Player));
                pd.AddHint(GetServerInfoHint(ev.Player));
                pd.AddHint(GetMapInfoHint(ev.Player));
            }
            else
            {
                pd.AddHint(GetClockHint(ev.Player));
                pd.AddHint(GetTpsHint(ev.Player));
                pd.AddHint(GetRoundTimeHint(ev.Player));
                pd.AddHint(GetPlayerInfoHint(ev.Player));
            }
        }

        private Hint GetClockHint(Player player)
        {
            if (!clockHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (!Options.ShouldShow(Config.ClockVisual, p) || !Config.EnableClock)
                            return string.Empty;
                        string timerColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);
                        return Config.Clock
                            .Replace("{color}", timerColor)
                            .Replace("{time}", utc.ToString("HH:mm"));
                    },
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.ClockXCordinate,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                clockHints[player] = hint;
            }
            return hint;
        }
        private Hint GetTpsHint(Player player)
        {
            if (!tpsHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (!Options.ShouldShow(Config.TpsVisual, p) || !Config.EnableTps)
                            return string.Empty;
                        int tps = (int)Server.Tps;
                        int maxTps = (int)Server.MaxTps;
                        string tpsColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        return Config.Tps
                            .Replace("{color}", tpsColor)
                            .Replace("{tps}", tps.ToString())
                            .Replace("{maxTps}", maxTps.ToString());
                    },
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.TpsXCordinate,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                tpsHints[player] = hint;
            }
            return hint;
        }
        private Hint GetRoundTimeHint(Player player)
        {
            if (!roundTimeHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (!Options.ShouldShow(Config.RoundTimeVisual, p) || !Config.EnableRoundTime)
                            return string.Empty;
                        TimeSpan elapsed = Round.ElapsedTime;
                        string elapsedFormatted = elapsed.ToString(@"mm\:ss");
                        string elapsedColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        return Config.RoundTime
                            .Replace("{color}", elapsedColor)
                            .Replace("{round_time}", elapsedFormatted);
                    },
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.RoundTimeXCordinate,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                roundTimeHints[player] = hint;
            }
            return hint;
        }
        private Hint GetPlayerInfoHint(Player player)
        {
            if (!playerInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (p.Role is SpectatorRole)
                            return string.Empty;

                        string roleColor = "#" + ColorUtility.ToHtmlStringRGB(p.Role.Color);
                        string nickname = p.Nickname;
                        if (nickname.Length > 20)
                            nickname = nickname.Substring(0, 20) + "...";
                        uint id = (uint)p.Id;
                        string role = Translation.GetRoleDisplayName(p);
                        string coloredRole = $"<color={roleColor}>{role}</color>";
                        int kills = GetKills(p);

                        return Config.PlayerHud
                            .Replace("{nickname}", nickname)
                            .Replace("{id}", id.ToString())
                            .Replace("{role}", coloredRole)
                            .Replace("{kills}", kills.ToString());
                    },
                    FontSize = 33,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                playerInfoHints[player] = hint;
            }
            return hint;
        }
        private Hint GetSpectatorPlayerInfoHint(Player player)
        {
            if (!spectatorPlayerInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (!(p.Role is SpectatorRole spectatorRole))
                            return string.Empty;
                        Player observed = spectatorRole.SpectatedPlayer;
                        if (observed == null)
                            return string.Empty;
                        string observedRoleColor = "#" + ColorUtility.ToHtmlStringRGB(observed.Role.Color);
                        string observedNickname = observed.Nickname;
                        if (observedNickname.Length > 14)
                            observedNickname = observedNickname.Substring(0, 14) + "...";
                        uint observedId = (uint)observed.Id;
                        string observedRole = Translation.GetRoleDisplayName(observed);
                        string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                        int observedKills = GetKills(observed);

                        return Config.SpectatorHud
                            .Replace("{nickname}", observedNickname)
                            .Replace("{id}", observedId.ToString())
                            .Replace("{role}", coloredObservedRole)
                            .Replace("{kills}", observedKills.ToString());
                    },
                    FontSize = 33,
                    YCoordinate = 1050,
                    Alignment = HintAlignment.Center,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                spectatorPlayerInfoHints[player] = hint;
            }
            return hint;
        }
        private Hint GetServerInfoHint(Player player)
        {
            if (!serverInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        if (!(p.Role is SpectatorRole))
                            return string.Empty;
                        int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                        int maxPlayers = Server.MaxPlayerCount;
                        int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);

                        return Config.SpectatorServerInfo
                            .Replace("{players}", totalPlayers.ToString())
                            .Replace("{maxPlayers}", maxPlayers.ToString())
                            .Replace("{spectators}", spectators.ToString());
                    },
                    FontSize = 27,
                    YCoordinate = 1000,
                    Alignment = HintAlignment.Left,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                serverInfoHints[player] = hint;
            }
            return hint;
        }
        private Hint GetMapInfoHint(Player player)
        {
            if (!mapInfoHints.TryGetValue(player, out var hint))
            {
                hint = new Hint
                {
                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        int engaged = Generator.List.Count(g => g.IsEngaged);
                        int maxGenerators = 3;
                        bool isArmed = Warhead.Status == WarheadStatus.Armed;
                        string warheadStatus = isArmed ? "Armed" : "Not Armed";
                        string warheadColor = isArmed ? "red" : "green";
                        return Config.SpectatorMapInfo
                            .Replace("{engaged}", engaged.ToString())
                            .Replace("{maxGenerators}", maxGenerators.ToString())
                            .Replace("{warheadColor}", warheadColor)
                            .Replace("{warheadStatus}", warheadStatus);
                    },
                    FontSize = 27,
                    YCoordinate = 1000,
                    Alignment = HintAlignment.Right,
                    SyncSpeed = HintSyncSpeed.Fast
                };
                mapInfoHints[player] = hint;
            }
            return hint;
        }

        private void RemoveHints(Player player)
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

        private void RemoveAllHints()
        {
            foreach (var player in Player.List.ToList())
                RemoveHints(player);
        }

        // Kill Counter handler
        private void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                foreach (var player in Player.List)
                {
                    if (player.Role.Type == RoleTypeId.Scp106)
                    {
                        Player killer = player;
                        if (playerKills.ContainsKey(killer))
                        {
                            playerKills[killer]++;
                        }
                        else
                        {
                            playerKills[killer] = 1;
                        }
                    }
                }
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                {
                    playerKills[killer]++;
                }
                else
                {
                    playerKills[killer] = 1;
                }
            }
        }
        public static int GetKills(Player player)
        {
            return playerKills.TryGetValue(player, out int kills) ? kills : 0;
        }
    }
}