using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.Spectating;
using System.Collections.Generic;
using System.Linq;
using MEC;

namespace UltimateHUD
{
    public static class EventHandlers
    {
        public static readonly Dictionary<Player, int> Kills = new();
        public static int GetKills(Player player) => player != null && Kills.TryGetValue(player, out var k) ? k : 0;

        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;

            LabApi.Events.Handlers.PlayerEvents.Joined += OnVerified;
            LabApi.Events.Handlers.PlayerEvents.Left += OnLeft;
            LabApi.Events.Handlers.PlayerEvents.Death += OnDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole += OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.ChangedSpectator += OnChangedSpectatedPlayer;
            LabApi.Events.Handlers.PlayerEvents.ChangedNickname += OnChangedNickname;

            LabApi.Events.Handlers.PlayerEvents.ShotWeapon += OnShot;
            LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon += OnReloaded;
            LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon += OnUnloaded;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem += OnChangedItem;

            LabApi.Events.Handlers.PlayerEvents.InteractingWarheadLever += OnWarheadLever;
            LabApi.Events.Handlers.WarheadEvents.Starting += OnWarheadStarting;
            LabApi.Events.Handlers.WarheadEvents.Stopping += OnWarheadStopping;
            LabApi.Events.Handlers.WarheadEvents.Detonated += OnWarheadDetonated;

            LabApi.Events.Handlers.ServerEvents.GeneratorActivating += OnGeneratorActivating;
        }

        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;

            LabApi.Events.Handlers.PlayerEvents.Joined -= OnVerified;
            LabApi.Events.Handlers.PlayerEvents.Left -= OnLeft;
            LabApi.Events.Handlers.PlayerEvents.Death -= OnDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole -= OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.ChangedSpectator -= OnChangedSpectatedPlayer;
            LabApi.Events.Handlers.PlayerEvents.ChangedNickname -= OnChangedNickname;

            LabApi.Events.Handlers.PlayerEvents.ShotWeapon -= OnShot;
            LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon -= OnReloaded;
            LabApi.Events.Handlers.PlayerEvents.UnloadedWeapon -= OnUnloaded;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem -= OnChangedItem;

            LabApi.Events.Handlers.PlayerEvents.InteractingWarheadLever -= OnWarheadLever;
            LabApi.Events.Handlers.WarheadEvents.Starting -= OnWarheadStarting;
            LabApi.Events.Handlers.WarheadEvents.Stopping -= OnWarheadStopping;
            LabApi.Events.Handlers.WarheadEvents.Detonated -= OnWarheadDetonated;

            LabApi.Events.Handlers.ServerEvents.GeneratorActivating -= OnGeneratorActivating;
        }

        /// <summary>
        /// Handles the reset player kill counts.
        /// </summary>
        private static void OnWaitingForPlayers()
        {
            Kills.Clear();
        }

        /// <summary>
        /// Handles the round end event to clear all hints.
        /// </summary>
        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.ReadyList)
                Hints.RemoveAll(p);
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        private static void OnVerified(PlayerJoinedEventArgs ev)
        {
            if (ev.Player == null || !Round.IsRoundStarted)
                return;

            Hints.RefreshAll(ev.Player);
            RefreshAllSpectatorServerInfo();
        }


        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        private static void OnLeft(PlayerLeftEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Kills.Remove(ev.Player);
            RefreshAllSpectatorServerInfo();
        }

        /// <summary>
        /// Handles the role change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        private static void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Player.IsDestroyed)
                    return;

                Hints.RefreshAll(ev.Player);

                RefreshAllSpectatorServerInfo();

                Hints.RefreshSpectatorList(ev.Player);
            });
        }

        /// <summary>
        /// Refreshes the hints for the spectated and spectating player when a player changes their spectated target.
        /// </summary>
        private static void OnChangedSpectatedPlayer(PlayerChangedSpectatorEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Player.IsDestroyed)
                    return;

                RefreshSpectatorListsForTargets(ev.OldTarget, ev.NewTarget);
                Hints.RefreshSpectatorPlayerInfo(ev.Player);
            });
        }

        /// <summary>
        /// Handles the nickname change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        private static void OnChangedNickname(PlayerChangedNicknameEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Player.IsDestroyed)
                    return;

                Hints.RefreshAll(ev.Player);

                RefreshAllSpectatorServerInfo();

                Hints.RefreshSpectatorList(ev.Player);
            });
        }

        /// <summary>
        /// Handles the player death event to update kill counts and hints for players, especially for SCP-106 kills in the Pocket Dimension.
        /// Refreshes the player info hints for the killer  and spectators if they are spectating the killer to update kill counter.
        /// </summary>
        private static void OnDied(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                AddKill(ev.Attacker);
                Hints.RefreshPlayerInfo(ev.Attacker);

                foreach (var spec in Player.ReadyList.Where(p => p.RoleBase is SpectatorRole && p.CurrentlySpectating == ev.Attacker))
                    Hints.RefreshSpectatorPlayerInfo(spec);
            }
        }

        private static void AddKill(Player killer)
        {
            if (killer == null)
                return;

            if (Kills.TryGetValue(killer, out var v))
                Kills[killer] = v + 1;
            else
                Kills[killer] = 1;
        }

        /// <summary>
        /// Refreshes the ammo hint when a player shoots a firearm.
        /// </summary>
        private static void OnShot(PlayerShotWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RefreshAmmo(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player reloads a firearm.
        /// </summary>
        private static void OnReloaded(PlayerReloadedWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RefreshAmmo(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player unloads a firearm.
        /// </summary>
        private static void OnUnloaded(PlayerUnloadedWeaponEventArgs ev)
        {
            if (ev.FirearmItem == null)
                return;

            Hints.RefreshAmmo(ev.Player);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player changes their item, specifically for firearms.
        /// </summary>
        private static void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (ev.NewItem is FirearmItem || ev.OldItem is FirearmItem)
                Hints.RefreshAmmo(ev.Player);
        }

        /// <summary>
        /// Refreshes the map info hint for spectators.
        /// </summary>
        private static void OnWarheadLever(PlayerInteractingWarheadLeverEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadStarting(WarheadStartingEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadStopping(WarheadStoppingEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadDetonated(WarheadDetonatedEventArgs ev) => RefreshAllSpectatorMapInfo();
        private static void OnGeneratorActivating(GeneratorActivatingEventArgs ev) => DelayedMapUpdate();
        private static void DelayedMapUpdate(float delay = 0.1f) => Timing.CallDelayed(delay, RefreshAllSpectatorMapInfo);

        // ========== Helpers ==========

        private static void RefreshAllSpectatorServerInfo()
        {
            foreach (var spec in Player.ReadyList.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RefreshSpectatorServerInfo(spec);
            }
        }

        private static void RefreshAllSpectatorMapInfo()
        {
            foreach (var spec in Player.ReadyList.Where(p => p.RoleBase is SpectatorRole))
            {
                Hints.RefreshSpectatorMapInfo(spec);
            }
        }

        private static void RefreshSpectatorListsForTargets(Player oldTarget, Player newTarget)
        {
            if (oldTarget != null)
                Hints.RefreshSpectatorList(oldTarget);

            if (newTarget != null && newTarget != oldTarget)
                Hints.RefreshSpectatorList(newTarget);
        }
    }
}