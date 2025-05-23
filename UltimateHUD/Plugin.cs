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

namespace UltimateHUD
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "UltimateHUD";
        public override string Author => "Vretu";
        public override string Prefix => "UltimateHud";
        public override Version Version => new Version(1, 0, 0);

        private readonly Dictionary<Player, Hint> timeHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> tpsHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> playerInfoHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> spectatorPlayerInfoHints = new Dictionary<Player, Hint>();

        private readonly Dictionary<Player, Hint> mapInfoHints = new Dictionary<Player, Hint>();

        private Timer timer;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

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
            if (playerInfoHints.TryGetValue(player, out var oldPlayerHint)) pd.RemoveHint(oldPlayerHint);
            if (spectatorPlayerInfoHints.TryGetValue(player, out var oldSpecHint)) pd.RemoveHint(oldSpecHint);
            if (mapInfoHints.TryGetValue(player, out var oldMapHint)) pd.RemoveHint(oldMapHint);

            string timerColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
            DateTime utc2 = DateTime.UtcNow.AddHours(2);
            string timeText = Config.Clock
            .Replace("{color}", timerColor)
            .Replace("{time}", utc2.ToString("HH:mm:ss"));

            var timeHint = new Hint
            {
                Text = timeText,
                FontSize = 25,
                YCoordinate = 20,
                Alignment = HintAlignment.Left
            };
            pd.AddHint(timeHint);
            timeHints[player] = timeHint;
            pd.RemoveAfter(timeHint, 1.1f);

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
                Alignment = HintAlignment.Center
            };
            pd.AddHint(tpsHint);
            tpsHints[player] = tpsHint;
            pd.RemoveAfter(tpsHint, 1.1f);

            // HUD for Spectators
            if (player.Role is SpectatorRole spectatorRole)
            {
                Player observed = spectatorRole.SpectatedPlayer;
                if (observed != null)
                {
                    string observedRoleColor = "#" + ColorUtility.ToHtmlStringRGB(observed.Role.Color);
                    string observedNickname = observed.Nickname;
                    uint observedId = (uint)observed.Id;
                    string observedRole = observed.Role.Type.ToString();
                    string coloredObservedRole = $"<color={observedRoleColor}>{observedRole}</color>";
                    string observedInfoText = Config.SpectatorHud
                    .Replace("{nickname}", observedNickname)
                    .Replace("{id}", observedId.ToString())
                    .Replace("{role}", coloredObservedRole);

                    var specHint = new Hint
                    {
                        Text = observedInfoText,
                        FontSize = 35,
                        YCoordinate = 1050,
                        Alignment = HintAlignment.Center
                    };
                    pd.AddHint(specHint);
                    spectatorPlayerInfoHints[player] = specHint;
                    pd.RemoveAfter(specHint, 1.1f);
                }

                int engaged = Generator.List.Count(g => g.IsEngaged);
                int maxGenerators = 3;

                bool isArmed = Warhead.Status == WarheadStatus.Armed;
                string warheadStatus = isArmed ? "Armed" : "Not Armed";
                string warheadColor = isArmed ? "red" : "green";

                string mapInfoText = Config.SpectatorInfo
                .Replace("{engaged}", engaged.ToString())
                .Replace("{maxGenerators}", maxGenerators.ToString())
                .Replace("{warheadColor}", warheadColor)
                .Replace("{warheadStatus}", warheadStatus);

                var mapHint = new Hint
                {
                    Text = mapInfoText,
                    FontSize = 27,
                    YCoordinate = 1000,
                    Alignment = HintAlignment.Center
                };
                pd.AddHint(mapHint);
                mapInfoHints[player] = mapHint;
                pd.RemoveAfter(mapHint, 1.1f);

                if (playerInfoHints.ContainsKey(player))
                {
                    pd.RemoveHint(playerInfoHints[player]);
                    playerInfoHints.Remove(player);
                }
                return;
            }

            // HUD for Players
            string roleColor = "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
            string nickname = player.Nickname;
            uint id = (uint)player.Id;
            string role = player.Role.Type.ToString();
            string coloredRole = $"<color={roleColor}>{role}</color>";

            string playerInfoText = Config.PlayerHud
            .Replace("{nickname}", nickname)
            .Replace("{id}", id.ToString())
            .Replace("{role}", coloredRole);

            var playerInfoHint = new Hint
            {
                Text = playerInfoText,
                FontSize = 35,
                YCoordinate = 1050,
                Alignment = HintAlignment.Center
            };
            pd.AddHint(playerInfoHint);
            playerInfoHints[player] = playerInfoHint;
            pd.RemoveAfter(playerInfoHint, 1.1f);

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

            foreach (var kvp in mapInfoHints)
            {
                var pd = PlayerDisplay.Get(kvp.Key.ReferenceHub);
                pd.RemoveHint(kvp.Value);
            }
            mapInfoHints.Clear();
        }
    }
}