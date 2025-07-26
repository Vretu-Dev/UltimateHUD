using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using MEC;

namespace UltimateHUD
{
    public static class EventHandlers
    {
        private static readonly Dictionary<Player, int> playerKills = new();

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Left += OnLeftPlayer;
            Exiled.Events.Handlers.Player.Verified += OnVerifiedPlayer;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += OnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Player.Shot += OnPlayerShot;
            Exiled.Events.Handlers.Player.ReloadedWeapon += OnPlayerReloaded;
            Exiled.Events.Handlers.Player.UnloadedWeapon += OnPlayerUnloaded;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Detonated += OnDetonated;
            Exiled.Events.Handlers.Warhead.Starting += OnStarting;
            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;
            Exiled.Events.Handlers.Map.GeneratorActivating += OnGeneratorActivating;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Left -= OnLeftPlayer;
            Exiled.Events.Handlers.Player.Verified -= OnVerifiedPlayer;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= OnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Player.Shot -= OnPlayerShot;
            Exiled.Events.Handlers.Player.ReloadedWeapon -= OnPlayerReloaded;
            Exiled.Events.Handlers.Player.UnloadedWeapon -= OnPlayerUnloaded;
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Detonated -= OnDetonated;
            Exiled.Events.Handlers.Warhead.Starting -= OnStarting;
            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;
            Exiled.Events.Handlers.Map.GeneratorActivating -= OnGeneratorActivating;
        }

        /// <summary>
        /// Handles the round end event to clear all hints and reset player kill counts.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Hints.RemoveAllHints();
            playerKills.Clear();
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnVerifiedPlayer(VerifiedEventArgs ev)
        {
            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RefreshServerInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnLeftPlayer(LeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player.ReferenceHub);

            playerKills.Remove(ev.Player);

            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RefreshServerInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Handles the role change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                Hints.RefreshHints(ev.Player.ReferenceHub);

                if (ev.NewRole == RoleTypeId.Spectator)
                {
                    foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                    {
                        Hints.RefreshServerInfo(spectator.ReferenceHub);
                    }
                }
            });
        }

        /// <summary>
        /// Handles the player death event to update kill counts and hints for players, especially for SCP-106 kills in the Pocket Dimension.
        /// Refreshes the player info hints for the killer  and spectators if they are spectating the killer to update kill counter.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerDied(DiedEventArgs ev)
        {
            // Kill counter logic remains unchanged
            if (ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                foreach (var player in Player.List)
                {
                    if (player.Role.Type == RoleTypeId.Scp106)
                    {
                        Player killer = player;

                        if (playerKills.ContainsKey(killer))
                            playerKills[killer]++;
                        else
                            playerKills[killer] = 1;

                        Hints.RefreshPlayerInfo(killer.ReferenceHub);
                    }
                }
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                    playerKills[killer]++;
                else
                    playerKills[killer] = 1;

                Hints.RefreshPlayerInfo(killer.ReferenceHub);
            }

            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                var spectated = ((SpectatorRole)spectator.Role).SpectatedPlayer;

                if (spectated != null && spectated == ev.Attacker)
                    Hints.RefreshSpectatorPlayerInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Gets the number of kills a player has made during the round.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetKills(Player player)
        {
            return playerKills.TryGetValue(player, out int kills) ? kills : 0;
        }

        /// <summary>
        /// Refreshes the hints for the spectated and spectating player when a player changes their spectated target.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget == null || ev.OldTarget == null || ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                Hints.RefreshSpectatingPlayers(ev.NewTarget.ReferenceHub);
                Hints.RefreshSpectatingPlayers(ev.OldTarget.ReferenceHub);
                Hints.RefreshSpectatorPlayerInfo(ev.Player.ReferenceHub);
            });
        }

        /// <summary>
        /// Refreshes the ammo hint when a player shoots a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerShot(ShotEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player reloads a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerReloaded(ReloadedWeaponEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player unloads a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerUnloaded(UnloadedWeaponEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player changes their item, specifically for firearms.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (ev.Item is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);

            if (ev.OldItem is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the lever status is changing.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is starting.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnStarting(StartingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is detonated.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnDetonated()
        {
            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RefreshMapInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is stopping.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnStopping(StoppingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when a generator is activating.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnGeneratorActivating(GeneratorActivatingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }
    }
}