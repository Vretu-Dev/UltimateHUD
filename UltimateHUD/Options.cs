﻿using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API;
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

        public static string GetRoleDisplayName(this Translations config, Player player)
        {
            var customRole = player.GetCustomRoles().FirstOrDefault();

            if (customRole != null)
            {
                var translation = config.CustomGameRoles.FirstOrDefault(r => r.CustomRole == customRole.Name)?.Name;
                return translation ?? customRole.Name;
            }

            var normalTranslation = config.GameRoles.FirstOrDefault(r => r.Role == player.Role.Type)?.Name;

            return normalTranslation ?? player.Role.Type.ToString();
        }
    }
}
