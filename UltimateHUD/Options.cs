using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API;
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
            if (Visual == "gameplay" && player.Role is not SpectatorRole)
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

        public static string GetWeaponDisplayName(this Translations config, Firearm firearm)
        {
            var weaponTranslation = config.WeaponName.FirstOrDefault(w => w.Weapon == firearm.FirearmType);

            return weaponTranslation?.Name ?? firearm.FirearmType.ToString();
        }

        public static string GetRoleColor(Player player)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(player.Role.Color);
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
