using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using HintServiceMeow.Core.Utilities;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace UltimateHUD
{
    public static class EventHandlers
    {
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

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
            Exiled.Events.Handlers.Warhead.Detonating += OnDetonated;
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
            Exiled.Events.Handlers.Warhead.Detonating -= OnDetonated;
            Exiled.Events.Handlers.Warhead.Starting -= OnStarting;
            Exiled.Events.Handlers.Warhead.Stopping -= OnStopping;
            Exiled.Events.Handlers.Map.GeneratorActivating -= OnGeneratorActivating;
            Hints.RemoveAllHints();
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Hints.RemoveAllHints();
            playerKills.Clear();
        }

        private static void OnVerifiedPlayer(VerifiedEventArgs ev)
        {
            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RemoveServerInfoHint(spectator);
                Hints.AddServerInfoHint(spectator);
            }
        }

        private static void OnLeftPlayer(LeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player);
            playerKills.Remove(ev.Player);

            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                Hints.RemoveServerInfoHint(spectator);
                Hints.AddServerInfoHint(spectator);
            }
        }

        private static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            PlayerDisplay pd = PlayerDisplay.Get(ev.Player);

            Hints.RemoveHints(ev.Player);

            Timing.CallDelayed(Plugin.Instance.Config.RefreshTime, () =>
            {
                Hints.AddHints(ev.Player);

                if (ev.NewRole == RoleTypeId.Spectator)
                {
                    foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                    {
                        Hints.RemoveServerInfoHint(spectator);
                        Hints.AddServerInfoHint(spectator);
                    }
                }
            });
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
                            playerKills[killer]++;
                        else
                            playerKills[killer] = 1;

                        Hints.RemovePlayerInfoHint(killer);
                        Hints.AddPlayerInfoHint(killer);

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

                Hints.RemovePlayerInfoHint(killer);
                Hints.AddPlayerInfoHint(killer);
            }

            foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
            {
                var spectated = ((SpectatorRole)spectator.Role).SpectatedPlayer;
                if (spectated != null && spectated == ev.Attacker)
                {
                    Hints.RemoveSpectatorPlayerInfoHint(spectator);
                    Hints.AddSpectatorPlayerInfoHint(spectator);
                }
            }
        }
        public static int GetKills(Player player)
        {
            return playerKills.TryGetValue(player, out int kills) ? kills : 0;
        }

        private static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
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

        private static void OnPlayerShot(ShotEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        private static void OnPlayerReloaded(ReloadedWeaponEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        private static void OnPlayerUnloaded(UnloadedWeaponEventArgs ev)
        {
            if (ev.Item is not Firearm)
                return;

            Hints.RemoveAmmoHint(ev.Player);
            Hints.AddAmmoHint(ev.Player);
        }

        private static void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (ev.Item is Firearm)
                Hints.AddAmmoHint(ev.Player);
            if (ev.OldItem is Firearm)
                Hints.RemoveAmmoHint(ev.Player);
        }

        private static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }

        private static void OnStarting(StartingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }

        private static void OnDetonated(DetonatingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }

        private static void OnStopping(StoppingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }

        private static void OnGeneratorActivating(GeneratorActivatingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var spectator in Player.List.Where(p => p.Role is SpectatorRole))
                {
                    Hints.RemoveMapInfoHint(spectator);
                    Hints.AddMapInfoHint(spectator);
                }
            });
        }
    }
}