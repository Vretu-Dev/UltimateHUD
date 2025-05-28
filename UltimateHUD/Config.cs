using HintServiceMeow.Core.Enum;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace UltimateHUD
{
    public class Config
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public HintSyncSpeed HintSyncSpeed { get; set; } = HintSyncSpeed.Fast;

        [Description("Clock Settings:")]
        public bool EnableClock { get; set; } = true;
        public string Clock { get; set; } = "<color={color}><b>Time:</b> {time}</color>";
        [Description("UTC Time Zone | 2 = UTC+2")]
        public int TimeZone { get; set; } = 2;
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string ClockVisual { get; set; } = "BOTH";
        public int ClockXCordinate { get; set; } = -480;

        [Description("TPS Settings:")]
        public bool EnableTps { get; set; } = true;
        public string Tps { get; set; } = "<color={color}><b>TPS:</b> {tps}/{maxTps}</color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string TpsVisual { get; set; } = "BOTH";
        public int TpsXCordinate { get; set; } = -60;

        [Description("ROUND TIME Settings:")]
        public bool EnableRoundTime { get; set; } = true;
        public string RoundTime { get; set; } = "<color={color}><b>Round Time:</b> {round_time}</color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string RoundTimeVisual { get; set; } = "BOTH";
        public int RoundTimeXCordinate { get; set; } = 400;

        [Description("HUD Hints:")]
        public string PlayerHud { get; set; } = "<color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public string SpectatorHud { get; set; } = "<color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public string SpectatorMapInfo { get; set; } = "<b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color>";
        public string SpectatorServerInfo { get; set; } = "<b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color>";

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
    }
}