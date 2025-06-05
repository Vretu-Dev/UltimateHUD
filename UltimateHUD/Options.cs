using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.Spectating;
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
            if (Visual == "gameplay" && !(player.RoleBase is SpectatorRole))
                return true;
            if (Visual == "spectator" && player.RoleBase is SpectatorRole)
                return true;

            return false;
        }

        public static string GetRoleDisplayName(this Config config, Player player)
        {
            var normalTranslation = config.GameRoles.FirstOrDefault(r => r.Role == player.Role)?.Name;

            return normalTranslation ?? player.Role.ToString();
        }

        public static string GetRoleColor(Player player)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(player.RoleBase.RoleColor);
        }
    }
}
