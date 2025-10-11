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
        public static readonly Dictionary<string, int> Kills = new();
        public static int GetKills(Player player) => player != null && Kills.TryGetValue(player.UserId, out var k) ? k : 0;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += OnChangingSpectatedPlayer;

            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.ReloadedWeapon += OnReloaded;
            Exiled.Events.Handlers.Player.UnloadedWeapon += OnUnloaded;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += OnWarheadLever;
            Exiled.Events.Handlers.Warhead.Starting += OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping += OnWarheadStopping;
            Exiled.Events.Handlers.Warhead.Detonated += OnWarheadDetonated;

            Exiled.Events.Handlers.Map.GeneratorActivating += OnGeneratorActivating;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= OnChangingSpectatedPlayer;

            Exiled.Events.Handlers.Player.Shot -= OnShot;
            Exiled.Events.Handlers.Player.ReloadedWeapon -= OnReloaded;
            Exiled.Events.Handlers.Player.UnloadedWeapon -= OnUnloaded;
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= OnWarheadLever;
            Exiled.Events.Handlers.Warhead.Starting -= OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping -= OnWarheadStopping;
            Exiled.Events.Handlers.Warhead.Detonated -= OnWarheadDetonated;

            Exiled.Events.Handlers.Map.GeneratorActivating -= OnGeneratorActivating;
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
            foreach (var p in Player.List)
                Hints.RemoveAll(p);
        }

        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        private static void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player == null || Round.IsLobby)
                return;

            Hints.RefreshAll(ev.Player);
            RefreshAllSpectatorServerInfo();
        }


        /// <summary>
        /// Refreshes the server info hint for all spectators to ensure update players count and spectators count.
        /// </summary>
        private static void OnLeft(LeftEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Kills.Remove(ev.Player.UserId);
            RefreshAllSpectatorServerInfo();
        }

        /// <summary>
        /// Handles the role change event to refresh hints for the player and Server Info hint for all spectators.
        /// </summary>
        private static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (!ev.Player.IsConnected)
                    return;

                Hints.RefreshAll(ev.Player);

                RefreshAllSpectatorServerInfo();

                Hints.RefreshSpectatorList(ev.Player.ReferenceHub);
            });
        }

        /// <summary>
        /// Refreshes the hints for the spectated and spectating player when a player changes their spectated target.
        /// </summary>
        private static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.Player == null)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (!ev.Player.IsConnected)
                    return;

                RefreshSpectatorListsForTargets(ev.OldTarget, ev.NewTarget);
                Hints.RefreshSpectatorPlayerInfo(ev.Player.ReferenceHub);
            });
        }

        /// <summary>
        /// Handles the player death event to update kill counts and hints for players, especially for SCP-106 kills in the Pocket Dimension.
        /// Refreshes the player info hints for the killer  and spectators if they are spectating the killer to update kill counter.
        /// </summary>
        private static void OnDied(DiedEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                foreach (var scp106 in Player.List.Where(pl => pl.Role.Type == RoleTypeId.Scp106))
                {
                    AddKill(scp106);
                    Hints.RefreshPlayerInfo(scp106.ReferenceHub);
                }
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                AddKill(ev.Attacker);
                Hints.RefreshPlayerInfo(ev.Attacker.ReferenceHub);

                foreach (var spec in Player.List.Where(p => p.Role is SpectatorRole s && s.SpectatedPlayer == ev.Attacker))
                    Hints.RefreshSpectatorPlayerInfo(spec.ReferenceHub);
            }
        }

        private static void AddKill(Player killer)
        {
            if (killer == null)
                return;

            if (Kills.TryGetValue(killer.UserId, out var v))
                Kills[killer.UserId] = v + 1;
            else
                Kills[killer.UserId] = 1;
        }

        /// <summary>
        /// Refreshes the ammo hint when a player shoots a firearm.
        /// </summary>
        private static void OnShot(ShotEventArgs ev)
        {
            if (ev.Item is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player reloads a firearm.
        /// </summary>
        private static void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            if (ev.Item is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player unloads a firearm.
        /// </summary>
        private static void OnUnloaded(UnloadedWeaponEventArgs ev)
        {
            if (ev.Item is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the ammo hint when a player changes their item, specifically for firearms.
        /// </summary>
        private static void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (ev.Item is Firearm || ev.OldItem is Firearm)
                Hints.RefreshAmmo(ev.Player.ReferenceHub);
        }

        /// <summary>
        /// Refreshes the map info hint for spectators.
        /// </summary>
        private static void OnWarheadLever(ChangingLeverStatusEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadStarting(StartingEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadStopping(StoppingEventArgs ev) => DelayedMapUpdate();
        private static void OnWarheadDetonated() => RefreshAllSpectatorMapInfo();
        private static void OnGeneratorActivating(GeneratorActivatingEventArgs ev) => DelayedMapUpdate();
        private static void DelayedMapUpdate(float delay = 0.1f) => Timing.CallDelayed(delay, RefreshAllSpectatorMapInfo);

        // ========== Helpers ==========

        private static void RefreshAllSpectatorServerInfo()
        {
            foreach (var spec in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RefreshSpectatorServerInfo(spec.ReferenceHub);
            }
        }

        private static void RefreshAllSpectatorMapInfo()
        {
            foreach (var spec in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RefreshSpectatorMapInfo(spec.ReferenceHub);
            }
        }

        private static void RefreshSpectatorListsForTargets(Player oldTarget, Player newTarget)
        {
            if (oldTarget != null)
                Hints.RefreshSpectatorList(oldTarget.ReferenceHub);

            if (newTarget != null && newTarget != oldTarget)
                Hints.RefreshSpectatorList(newTarget.ReferenceHub);
        }
    }
}