using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
using System.Collections.Generic;

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
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Left -= OnLeftPlayer;
            Hints.RemoveAllHints();
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Hints.RemoveAllHints();
            playerKills.Clear();
        }

        private static void OnLeftPlayer(LeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player);
            playerKills.Remove(ev.Player);
        }

        private static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            PlayerDisplay pd = PlayerDisplay.Get(ev.Player);

            Hints.RemoveHints(ev.Player);

            if (ev.NewRole == RoleTypeId.Spectator)
            {
                pd.AddHint(Hints.GetClockHint(ev.Player));
                pd.AddHint(Hints.GetTpsHint(ev.Player));
                pd.AddHint(Hints.GetRoundTimeHint(ev.Player));
                pd.AddHint(Hints.GetSpectatorPlayerInfoHint(ev.Player));
                pd.AddHint(Hints.GetServerInfoHint(ev.Player));
                pd.AddHint(Hints.GetMapInfoHint(ev.Player));
            }
            else
            {
                pd.AddHint(Hints.GetClockHint(ev.Player));
                pd.AddHint(Hints.GetTpsHint(ev.Player));
                pd.AddHint(Hints.GetRoundTimeHint(ev.Player));
                pd.AddHint(Hints.GetPlayerInfoHint(ev.Player));
            }
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
