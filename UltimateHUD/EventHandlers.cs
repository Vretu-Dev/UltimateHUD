using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using MEC;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.Spectating;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.WarheadEvents;

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
            LabApi.Events.Handlers.PlayerEvents.ShotWeapon += OnPlayerShot;
            LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon += OnPlayerReloaded;
            LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon += OnPlayerUnloaded;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem += OnChangedItem;
            LabApi.Events.Handlers.PlayerEvents.InteractingWarheadLever += OnChangingLeverStatus;
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
            LabApi.Events.Handlers.PlayerEvents.ShotWeapon -= OnPlayerShot;
            LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon -= OnPlayerReloaded;
            LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon -= OnPlayerUnloaded;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem -= OnChangedItem;
            LabApi.Events.Handlers.PlayerEvents.InteractingWarheadLever -= OnChangingLeverStatus;
            LabApi.Events.Handlers.WarheadEvents.Detonated -= OnDetonated;
            LabApi.Events.Handlers.WarheadEvents.Starting -= OnStarting;
            LabApi.Events.Handlers.WarheadEvents.Stopping -= OnStopping;
            LabApi.Events.Handlers.ServerEvents.GeneratorActivating -= OnGeneratorActivating;
            Hints.RemoveAllHints();
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
        private static void OnVerifiedPlayer(PlayerJoinedEventArgs   ev)
        {
            foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RemoveServerInfoHint(spectator);
                Hints.AddServerInfoHint(spectator);
            }
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnLeftPlayer(PlayerLeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player);

            playerKills.Remove(ev.Player);

            foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RemoveServerInfoHint(spectator);
                Hints.AddServerInfoHint(spectator);
            }
        }

        /// <summary>
        /// Handles the role change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            Hints.RemoveHints(ev.Player);

            Timing.CallDelayed(0.1f, () =>
            {
                Hints.AddHints(ev.Player);

                if (ev.NewRole == RoleTypeId.Spectator)
                {
                    foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
                    {
                        Hints.RemoveServerInfoHint(spectator);
                        Hints.AddServerInfoHint(spectator);
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

                Hints.RemovePlayerInfoHint(killer);
                Hints.AddPlayerInfoHint(killer);
            }

            foreach (var spectator in Player.List.Where(p => p.Role == RoleTypeId.Spectator))
            {
                if (ev.Attacker != null && ev.Attacker == spectator.CurrentlySpectating)
                {
                    Hints.RemoveSpectatorPlayerInfoHint(spectator);
                    Hints.AddSpectatorPlayerInfoHint(spectator);
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

            Hints.RemoveSpectatingPlayerHint(ev.NewTarget);
            Hints.AddSpectatingPlayerHint(ev.NewTarget);

            Hints.RemoveSpectatingPlayerHint(ev.OldTarget);
            Hints.AddSpectatingPlayerHint(ev.OldTarget);

            Hints.RemoveSpectatorPlayerInfoHint(ev.Player);
            Hints.AddSpectatorPlayerInfoHint(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player shoots a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerShot(PlayerShotWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player reloads a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerReloaded(PlayerReloadedWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player unloads a firearm.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnPlayerUnloaded(PlayerUnloadedWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player changes their item, specifically for firearms.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (ev.NewItem is FirearmItem)
                Hints.AddAmmoHint(ev.Player);
            if (ev.OldItem is FirearmItem)
                Hints.RemoveAmmoHint(ev.Player);
        }

        /// <summary>
        /// Refreshes the map info hint for spectators when the lever status is changing.
        /// </summary>
        /// <param name="ev"></param>
        private static void OnChangingLeverStatus(PlayerInteractingWarheadLeverEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.RoleBase is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }

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
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
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
                Hints.RemoveMapInfoHint(spectator);
                Hints.AddMapInfoHint(spectator);
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
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
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
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }
    }
}