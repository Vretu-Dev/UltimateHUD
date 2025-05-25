using Exiled.API.Features;
using Exiled.API.Features.Roles;

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
    }
}
