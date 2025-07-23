using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.Spectating;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using MEC;

namespace UltimateHUD
{
    public static class EventHandlers
    {
        private static readonly Dictionary<Player, int> playerKills = new();

        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;
            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole += OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Left += OnLeftPlayer;
            LabApi.Events.Handlers.PlayerEvents.Joined += OnVerifiedPlayer;
            LabApi.Events.Handlers.PlayerEvents.ChangedSpectator += OnChangingSpectatedPlayer;
            //LabApi.Events.Handlers.PlayerEvents.ShotWeapon += OnPlayerShot;
            //LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon += OnPlayerReloaded;
            //LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon += OnPlayerUnloaded;
            //LabApi.Events.Handlers.PlayerEvents.ChangedItem += OnChangedItem;
            //LabApi.Events.Handlers.WarheadEvents.ChangingLever += OnChangingLeverStatus;
            LabApi.Events.Handlers.WarheadEvents.Detonated += OnDetonated;
            LabApi.Events.Handlers.WarheadEvents.Starting += OnStarting;
            LabApi.Events.Handlers.WarheadEvents.Stopping += OnStopping;
            LabApi.Events.Handlers.ServerEvents.GeneratorActivating += OnGeneratorActivating;
        }

        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole -= OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Left -= OnLeftPlayer;
            LabApi.Events.Handlers.PlayerEvents.Joined -= OnVerifiedPlayer;
            LabApi.Events.Handlers.PlayerEvents.ChangedSpectator -= OnChangingSpectatedPlayer;
            //LabApi.Events.Handlers.PlayerEvents.ShotWeapon -= OnPlayerShot;
            //LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon -= OnPlayerReloaded;
            //LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon -= OnPlayerUnloaded;
            //LabApi.Events.Handlers.PlayerEvents.ChangedItem -= OnChangedItem;
            //LabApi.Events.Handlers.WarheadEvents.ChangingLever -= OnChangingLeverStatus;
            LabApi.Events.Handlers.WarheadEvents.Detonated -= OnDetonated;
            LabApi.Events.Handlers.WarheadEvents.Starting -= OnStarting;
            LabApi.Events.Handlers.WarheadEvents.Stopping -= OnStopping;
            LabApi.Events.Handlers.ServerEvents.GeneratorActivating -= OnGeneratorActivating;
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
        private static void OnVerifiedPlayer(PlayerJoinedEventArgs ev)
        {
            foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RefreshServerInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnLeftPlayer(PlayerLeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player.ReferenceHub);

            playerKills.Remove(ev.Player);

            foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RefreshServerInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Handles the role change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                Hints.RefreshHints(ev.Player.ReferenceHub);

                if (ev.NewRole == RoleTypeId.Spectator)
                {
                    foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
                    {
                        Hints.RefreshServerInfo(spectator.ReferenceHub);
                    }
                }
            });
        }

        /// <summary>
        /// Refreshes the player info hints for spectators when a player dies.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                    playerKills[killer]++;
                else
                    playerKills[killer] = 1;

                Hints.RefreshPlayerInfo(killer.ReferenceHub);
            }

            foreach (var spectator in Player.List.Where(p => p.Role == RoleTypeId.Spectator))
            {
                if (ev.Attacker != null && ev.Attacker == spectator.CurrentlySpectating)
                {
                    Hints.RefreshSpectatorPlayerInfo(spectator.ReferenceHub);
                }
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
        private static void OnChangingSpectatedPlayer(PlayerChangedSpectatorEventArgs ev)
        {
            if (ev.NewTarget == null || ev.OldTarget == null || ev.Player == null)
                return;

            Hints.RefreshSpectatingPlayers(ev.NewTarget.ReferenceHub);
            Hints.RefreshSpectatingPlayers(ev.OldTarget.ReferenceHub);
            Hints.RefreshSpectatorPlayerInfo(ev.Player.ReferenceHub);
        }

        /* WAITING FOR FIREARM WRAPPER & WARHEAD CHANGING LEVER EVENT
        /// <summary>
        /// Refreshes the ammo hint when a player shoots a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerShot(PlayerShotWeaponEventArgs ev)
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
        */

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is starting.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnStarting(WarheadStartingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is detonated.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnDetonated(WarheadDetonatedEventArgs ev)
        {
            foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RefreshMapInfo(spectator.ReferenceHub);
            }
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the warhead is stopping.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnStopping(WarheadStoppingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
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
                foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
                {
                    Hints.RefreshMapInfo(spectator.ReferenceHub);
                }
            });
        }
    }
}