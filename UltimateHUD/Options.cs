using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles;
using System.Linq;

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
            if (Visual == "gameplay" && !(player.Role is SpectatorRole))
                return true;
            if (Visual == "spectator" && player.Role is SpectatorRole)
                return true;

            return false;
        }

        public static string GetRoleDisplayName(this Config config, RoleTypeId roleType)
        {
            return Plugin.Instance.Translation.GameRoles.FirstOrDefault(r => r.Role == roleType)?.Name ?? roleType.ToString();
        }
    }
}
