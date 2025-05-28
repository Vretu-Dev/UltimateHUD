using HintServiceMeow.Core.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System.Collections.Generic;

namespace UltimateHUD
{
    public static class EventHandlers
    {
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;
            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole += OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Left += OnLeftPlayer;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.ChangingRole -= OnChangingRole;
            LabApi.Events.Handlers.PlayerEvents.Left -= OnLeftPlayer;
            Hints.RemoveAllHints();
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Hints.RemoveAllHints();
            playerKills.Clear();
        }

        private static void OnLeftPlayer(PlayerLeftEventArgs ev)
        {
            Hints.RemoveHints(ev.Player);
            playerKills.Remove(ev.Player);
        }

        private static void OnChangingRole(PlayerChangingRoleEventArgs ev)
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
        private static void OnPlayerDied(PlayerDeathEventArgs ev)
        {
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
