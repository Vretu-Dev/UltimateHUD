using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace UltimateHUD
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Clock Settings:")]
        public bool EnableClock { get; set; } = true;
        public string Clock { get; set; } = "<space=-480><color={color}><size=25><b>Time:</b> {time}</size></color>";
        [Description("UTC Time Zone | 2 = UTC+2")]
        public int TimeZone { get; set; } = 2;
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string ClockVisual { get; set; } = "BOTH";
        public int ClockYCordinate { get; set; } = 980;

        [Description("TPS Settings:")]
        public bool EnableTps { get; set; } = true;
        public string Tps { get; set; } = "<space=-60><color={color}><size=25><b>TPS:</b> {tps}/{maxTps}</size></color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string TpsVisual { get; set; } = "BOTH";
        public int TpsYCordinate { get; set; } = 980;

        [Description("ROUND TIME Settings:")]
        public bool EnableRoundTime { get; set; } = true;
        public string RoundTime { get; set; } = "<space=400><color={color}><size=25><b>Round Time:</b> {round_time}</size></color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string RoundTimeVisual { get; set; } = "BOTH";
        public int RoundTimeYCordinate { get; set; } = 980;

        [Description("Player HUD Settings:")]
        public bool EnablePlayerHud { get; set; } = true;
        [Description("You can use {displayname} instead of {nickname}")]
        public string PlayerHud { get; set; } = "<size=33><color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color></size>";
        public int PlayerHudYCordinate { get; set; } = 15;
        
        [Description("Spectator List:")]
        public bool EnableSpectatorList { get; set; } = true;
        public string SpectatorListHeader { get; set; } = "<align=right><size=28><color={color}>👥 Spectators ({count})</color></size></align>";
        public string SpectatorListPlayers { get; set; } = "<align=right><size=28><color={color}>• {nickname}</color></size></align>";
        public List<RoleTypeId> HiddenForRoles { get; set; } = [RoleTypeId.Overwatch];
        public int SpectatorListYCordinate { get; set; } = 800;

        [Description("Ammo Counter:")]
        public bool EnableAmmoCounter { get; set; } = true;
        public string WeaponName { get; set; } = "<size=28><space=-900><color={color}>{weapon}</color> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt</size>";
        public string AmmoCounter { get; set; } = "<size=28><space=-900><b><color={color}>{current} / {max}</color></b> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt</size>";
        public int AmmoCounterYCordinate { get; set; } = 150;

        [Description("Spectator HUD Settings:")]
        public bool EnableSpectatorHud { get; set; } = true;
        [Description("You can use {displayname} instead of {nickname}")]
        public string SpectatorHud { get; set; } = "<size=33><color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color></size>";
        public int SpectatorHudYCordinate { get; set; } = 15;
        [Description("If true, will not show the spectated player's nickname when they are a skeleton (SCP-3114). This mimics the behavior of the base-game spectator UI.")]
        public bool HideSkeletonNickname { get; set; } = true;

        [Description("Spectator Map Info:")]
        public bool EnableSpectatorMapInfo { get; set; } = true;
        public string SpectatorMapInfo { get; set; } = "<space=650><size=27><b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color></size>";
        public int MapInfoYCordinate { get; set; } = 70;

        [Description("Spectator Server Info:")]
        public bool EnableSpectatorServerInfo { get; set; } = true;
        public string SpectatorServerInfo { get; set; } = "<space=-500><size=27><b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color></size>";
        public int ServerInfoYCordinate { get; set; } = 70;
    }
}