using System;
using System.Linq;
using UnityEngine;
using System.Timers;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using PlayerRoles;

namespace UltimateHUD
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "UltimateHUD";
        public override string Author => "Vretu";
        public override string Prefix => "UltimateHud";
        public override Version Version => new Version(2, 2, 0);
        public static Plugin Instance { get; private set; }

        private readonly Dictionary<Player, Hint> timeHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> tpsHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> roundTimeHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> playerInfoHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> serverInfoHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> mapInfoHints = new Dictionary<Player, Hint>();

        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

        private Timer timer;

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;

            timer?.Stop();
            timer?.Dispose();
            ClearAllHints();
            base.OnDisabled();
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            if (Round.IsStarted)
                AddOrUpdateHud(ev.Player);
        }

        private void OnRoundStarted()
        {
            timer = new Timer(Config.MsRefreshRate);
            timer.Elapsed += (s, e) => UpdateHudForAll();
            timer.Start();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            timer?.Stop();
            timer?.Dispose();
            ClearAllHints();
        }

        private void UpdateHudForAll()
        {
            foreach (var player in Player.List)
            {
                if (player.IsVerified)
                    AddOrUpdateHud(player);
            }
        }

        private void AddOrUpdateHud(Player player)
        {
            var pd = PlayerDisplay.Get(player.ReferenceHub);

            if (timeHints.TryGetValue(player, out var oldTimeHint)) pd.RemoveHint(oldTimeHint);
            if (tpsHints.TryGetValue(player, out var oldTpsHint)) pd.RemoveHint(oldTpsHint);
            if (roundTimeHints.TryGetValue(player, out var oldRoundTimeHint)) pd.RemoveHint(oldRoundTimeHint);
            if (playerInfoHints.TryGetValue(player, out var oldPlayerHint)) pd.RemoveHint(oldPlayerHint);
            if (spectatorPlayerInfoHints.TryGetValue(player, out var oldSpecHint)) pd.RemoveHint(oldSpecHint);
            if (serverInfoHints.TryGetValue(player, out var oldServerInfoHint)) pd.RemoveHint(oldServerInfoHint);
            if (mapInfoHints.TryGetValue(player, out var oldMapHint)) pd.RemoveHint(oldMapHint);

            // CLOCK HINT
            if (Options.ShouldShow(Config.ClockVisual, player) && Config.EnableClock)
            {
                string timerColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
                DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);
                string timeText = Config.Clock
                .Replace("{color}", timerColor)
                .Replace("{time}", utc.ToString("HH:mm"));

                var timeHint = new Hint
                {
                    Text = timeText,
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.ClockXCordinate
                };
                pd.AddHint(timeHint);
                timeHints[player] = timeHint;
                pd.RemoveAfter(timeHint, 1.1f);
            }

            // TPS HINT
            if (Options.ShouldShow(Config.TpsVisual, player) && Config.EnableTps)
            {
                int tps = (int)Server.Tps;
                int maxTps = (int)Server.MaxTps;
                string tpsColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
                string tpsText = Config.Tps
                .Replace("{color}", tpsColor)
                .Replace("{tps}", tps.ToString())
                .Replace("{maxTps}", maxTps.ToString());

                var tpsHint = new Hint
                {
                    Text = tpsText,
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.TpsXCordinate
                };
                pd.AddHint(tpsHint);
                tpsHints[player] = tpsHint;
                pd.RemoveAfter(tpsHint, 1.1f);
            }

            // ROUND TIME HINT
            if (Options.ShouldShow(Config.RoundTimeVisual, player) && Config.EnableRoundTime)
            {
                TimeSpan elapsed = Round.ElapsedTime;
                string elapsedFormatted = elapsed.ToString(@"mm\:ss");
                string elapsedColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
                string roundTimeText = Config.RoundTime
                .Replace("{color}", elapsedColor)
                .Replace("{round_time}", elapsedFormatted);

                var roundTimeHint = new Hint
                {
                    Text = roundTimeText,
                    FontSize = 25,
                    YCoordinate = 20,
                    XCoordinate = Config.RoundTimeXCordinate
                };
                pd.AddHint(roundTimeHint);
                roundTimeHints[player] = roundTimeHint;
                pd.RemoveAfter(roundTimeHint, 1.1f);
            }

            // HUD for Spectators
            if (player.Role is SpectatorRole spectatorRole)
            {
                Player observed = spectatorRole.SpectatedPlayer;

                if (observed != null)
                {
                    // MAIN HUD
                    string observedRoleColor = "#" + ColorUtility.ToHtmlStringRGB(observed.Role.Color);
                    string observedNickname = observed.Nickname;

                    if (observedNickname.Length > 14)
                        observedNickname = observedNickname.Substring(0, 14) + "...";

                    uint observedId = (uint)observed.Id;
                    string observedRole = Translation.GetRoleDisplayName(observed);
                    string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                    int observedKills = GetKills(observed);

                    string observedInfoText = Config.SpectatorHud
                    .Replace("{nickname}", observedNickname)
                    .Replace("{id}", observedId.ToString())
                    .Replace("{role}", coloredObservedRole)
                    .Replace("{kills}", observedKills.ToString());

                    var specHint = new Hint
                    {
                        Text = observedInfoText,
                        FontSize = 33,
                        YCoordinate = 1050,
                        Alignment = HintAlignment.Center
                    };
                    pd.AddHint(specHint);
                    spectatorPlayerInfoHints[player] = specHint;
                    pd.RemoveAfter(specHint, 1.1f);
                }

                // MAP INFO HINT
                int engaged = Generator.List.Count(g => g.IsEngaged);
                int maxGenerators = 3;

                bool isArmed = Warhead.Status == WarheadStatus.Armed;
                string warheadStatus = isArmed ? "Armed" : "Not Armed";
                string warheadColor = isArmed ? "red" : "green";

                string mapInfoText = Config.SpectatorMapInfo
                .Replace("{engaged}", engaged.ToString())
                .Replace("{maxGenerators}", maxGenerators.ToString())
                .Replace("{warheadColor}", warheadColor)
                .Replace("{warheadStatus}", warheadStatus);

                var mapHint = new Hint
                {
                    Text = mapInfoText,
                    FontSize = 27,
                    YCoordinate = 1000,
                    Alignment = HintAlignment.Right
                };
                pd.AddHint(mapHint);
                mapInfoHints[player] = mapHint;
                pd.RemoveAfter(mapHint, 1.1f);

                // Clear Player Info Hints if player is a spectator
                if (playerInfoHints.ContainsKey(player))
                {
                    pd.RemoveHint(playerInfoHints[player]);
                    playerInfoHints.Remove(player);
                }

                // SERVER INFO HINT
                int totalPlayers = Player.List.Count(p => !p.IsHost);
                int maxPlayers = Server.MaxPlayerCount;
                int spectators = Player.List.Count(p => p.Role is SpectatorRole && !p.IsHost);

                string serverInfoText = Config.SpectatorServerInfo
                    .Replace("{players}", totalPlayers.ToString())
                    .Replace("{maxPlayers}", maxPlayers.ToString())
                    .Replace("{spectators}", spectators.ToString());

                var serverInfoHint = new Hint
                {
                    Text = serverInfoText,
                    FontSize = 27,
                    YCoordinate = 1000,
                    Alignment = HintAlignment.Left
                };
                pd.AddHint(serverInfoHint);
                serverInfoHints[player] = serverInfoHint;
                pd.RemoveAfter(serverInfoHint, 1.1f);

                return;
            }

            // HUD for Players
            string roleColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
            string nickname = player.Nickname;

            if (nickname.Length > 20)
                nickname = nickname.Substring(0, 20) + "...";

            uint id = (uint)player.Id;
            string role = Translation.GetRoleDisplayName(player);
            string coloredRole = $"<color={roleColor}>{role}</color>";
            int kills = GetKills(player);

            string playerInfoText = Config.PlayerHud
            .Replace("{nickname}", nickname)
            .Replace("{id}", id.ToString())
            .Replace("{role}", coloredRole)
            .Replace("{kills}", kills.ToString());

            var playerInfoHint = new Hint
            {
                Text = playerInfoText,
                FontSize = 33,
                YCoordinate = 1050,
                Alignment = HintAlignment.Center
            };
            pd.AddHint(playerInfoHint);
            playerInfoHints[player] = playerInfoHint;
            pd.RemoveAfter(playerInfoHint, 1.1f);

            // Clear Spectator Hints if player is not a spectator
            if (spectatorPlayerInfoHints.TryGetValue(player, out var oldSpecHint2))
            {
                pd.RemoveHint(oldSpecHint2);
                spectatorPlayerInfoHints.Remove(player);
            }

            if (mapInfoHints.TryGetValue(player, out var oldMapHint2))
            {
                pd.RemoveHint(oldMapHint2);
                mapInfoHints.Remove(player);
            }

            if (serverInfoHints.TryGetValue(player, out var oldServerHint2))
            {
                pd.RemoveHint(oldServerHint2);
                serverInfoHints.Remove(player);
            }
        }

        private void ClearAllHints()
        {
            foreach (var kvp in timeHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            timeHints.Clear();

            foreach (var kvp in tpsHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            tpsHints.Clear();

            foreach (var kvp in roundTimeHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            roundTimeHints.Clear();

            foreach (var kvp in playerInfoHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            playerInfoHints.Clear();

            foreach (var kvp in spectatorPlayerInfoHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            spectatorPlayerInfoHints.Clear();

            foreach (var kvp in serverInfoHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            serverInfoHints.Clear();

            foreach (var kvp in mapInfoHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            mapInfoHints.Clear();
        }

        // Kill Counter handler
        private static void OnPlayerDied(DiedEventArgs ev)
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