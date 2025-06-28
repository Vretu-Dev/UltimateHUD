using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.Spectating;
using System;
using System.Linq;
using UnityEngine;

namespace UltimateHUD
{
    public static class Options
    {
        public static bool ShouldShow(string Visual, Player player)
        {
            if (string.IsNullOrEmpty(Visual))
                return true;

            Visual = Visual.ToLowerInvariant();

            if (Visual == "both")
                return true;
            if (Visual == "gameplay" && player.RoleBase is not SpectatorRole)
                return true;
            if (Visual == "spectator" && player.RoleBase is SpectatorRole)
                return true;

            return false;
        }

        public static string GetRoleDisplayName(this Translations config, Player player)
        {
            var normalTranslation = config.GameRoles.FirstOrDefault(r => r.Role == player.Role)?.Name;

            return normalTranslation ?? player.Role.ToString();
        }

        public static string GetRoleColor(Player player)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(player.RoleBase.RoleColor);
        }

        public static WarheadStatus GetCurrentWarheadStatus()
        {
            if (Warhead.IsDetonated)
                return WarheadStatus.Detonated;
            if (Warhead.IsDetonationInProgress)
                return WarheadStatus.InProgress;
            if (Warhead.LeverStatus == Warhead.IsLocked)
                return WarheadStatus.NotArmed;
            return WarheadStatus.Armed;
        }

        public static string GetWarheadStatusName(this Translations config, WarheadStatus status)
        {
            foreach (var w in config.WarheadStatuses)
            {
                if (w.Status == status)
                    return w.Name;
            }
            return "Unknown";
        }

        public static string GetWarheadStatusColor(this Translations config, WarheadStatus status)
        {
            foreach (var w in config.WarheadStatuses)
            {
                if (w.Status == status)
                    return w.Color;
            }
            return "white";
        }
    }
}
