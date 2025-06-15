using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;

namespace UltimateHUD
{
    public class RoleName
    {
        public RoleTypeId Role { get; set; }
        public string Name { get; set; }
    }

    public class CustomRoleName
    {
        public string CustomRole { get; set; }
        public string Name { get; set; }
    }

    public class WeaponName
    {
        public FirearmType Weapon { get; set; }
        public string Name { get; set; }
    }

    public class WarheadStatusName
    {
        public WarheadStatus Status { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
    public class Translations : ITranslation
    {
        public List<WarheadStatusName> WarheadStatuses { get; set; } = new List<WarheadStatusName>()
        {
            new WarheadStatusName { Status = WarheadStatus.Armed,      Name = "Armed",      Color = "red" },
            new WarheadStatusName { Status = WarheadStatus.NotArmed,   Name = "Not Armed",  Color = "green" },
            new WarheadStatusName { Status = WarheadStatus.InProgress, Name = "In Progress",Color = "orange" },
            new WarheadStatusName { Status = WarheadStatus.Detonated,  Name = "Detonated",  Color = "#8B0000" },
        };

        public List<RoleName> GameRoles { get; set; } = new List<RoleName>()
        {
            new RoleName { Role = RoleTypeId.Tutorial, Name = "Tutorial" },
            new RoleName { Role = RoleTypeId.ClassD, Name = "Class-D" },
            new RoleName { Role = RoleTypeId.Scientist, Name = "Scientist" },
            new RoleName { Role = RoleTypeId.FacilityGuard, Name = "Facility Guard" },
            new RoleName { Role = RoleTypeId.Filmmaker, Name = "Film Maker" },
            new RoleName { Role = RoleTypeId.Overwatch, Name = "Overwatch" },
            new RoleName { Role = RoleTypeId.NtfPrivate, Name = "MTF Private" },
            new RoleName { Role = RoleTypeId.NtfSergeant, Name = "MTF Sergeant" },
            new RoleName { Role = RoleTypeId.NtfSpecialist, Name = "MTF Specialist" },
            new RoleName { Role = RoleTypeId.NtfCaptain, Name = "MTF Captain" },
            new RoleName { Role = RoleTypeId.ChaosConscript, Name = "CI Conscript" },
            new RoleName { Role = RoleTypeId.ChaosRifleman, Name = "CI Rifleman" },
            new RoleName { Role = RoleTypeId.ChaosRepressor, Name = "CI Repressor" },
            new RoleName { Role = RoleTypeId.ChaosMarauder, Name = "CI Marauder" },
            new RoleName { Role = RoleTypeId.Scp049, Name = "SCP-049" },
            new RoleName { Role = RoleTypeId.Scp0492, Name = "SCP-049-2" },
            new RoleName { Role = RoleTypeId.Scp079, Name = "SCP-079" },
            new RoleName { Role = RoleTypeId.Scp096, Name = "SCP-096" },
            new RoleName { Role = RoleTypeId.Scp106, Name = "SCP-106" },
            new RoleName { Role = RoleTypeId.Scp173, Name = "SCP-173" },
            new RoleName { Role = RoleTypeId.Scp939, Name = "SCP-939" },
            new RoleName { Role = RoleTypeId.Scp3114, Name = "SCP-3114" },
            
        };

        public List<WeaponName> WeaponName { get; set; } = new List<WeaponName>()
        {
            new WeaponName { Weapon = FirearmType.Com15, Name = "COM-15" },
            new WeaponName { Weapon = FirearmType.Com18, Name = "COM-18" },
            new WeaponName { Weapon = FirearmType.E11SR, Name = "Epsilon" },
            new WeaponName { Weapon = FirearmType.FSP9, Name = "FSP-9" },
            new WeaponName { Weapon = FirearmType.Logicer, Name = "Logicer" },
            new WeaponName { Weapon = FirearmType.Revolver, Name = "Revolver" },
            new WeaponName { Weapon = FirearmType.AK, Name = "AK" },
            new WeaponName { Weapon = FirearmType.Shotgun, Name = "Shotgun" },
            new WeaponName { Weapon = FirearmType.Com45, Name = "COM-45" },
            new WeaponName { Weapon = FirearmType.ParticleDisruptor, Name = "Particle X3" },
            new WeaponName { Weapon = FirearmType.FRMG0, Name = "FR-MG-0" },
            new WeaponName { Weapon = FirearmType.A7, Name = "A7" },
            new WeaponName { Weapon = FirearmType.Scp127, Name = "SCP-127" }
        };

        public List<CustomRoleName> CustomGameRoles { get; set; } = new List<CustomRoleName>()
        {
            new CustomRoleName { CustomRole = "Serpents Hand Agent", Name = "Serpent Agent" },
            new CustomRoleName { CustomRole = "Serpents Hand Guardian", Name = "Serpent Leader" },
            new CustomRoleName { CustomRole = "Serpents Hand Enchanter", Name = "Serpent Specialist" }
        };
    }
}