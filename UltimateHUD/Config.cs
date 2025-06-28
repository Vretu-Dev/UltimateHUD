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

        [Description("Time after which the hint is refreshed when the player changes roles")]
        public float RefreshTime { get; set; } = 0.05f;

        [Description("Clock Settings:")]
        public bool EnableClock { get; set; } = true;
        public string Clock { get; set; } = "<color={color}><b>Time:</b> {time}</color>";
        [Description("UTC Time Zone | 2 = UTC+2")]
        public int TimeZone { get; set; } = 2;
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string ClockVisual { get; set; } = "BOTH";
        public int ClockXCordinate { get; set; } = -480;
        public int ClockYCordinate { get; set; } = 20;
        public int ClockFontSize { get; set; } = 25;

        [Description("TPS Settings:")]
        public bool EnableTps { get; set; } = true;
        public string Tps { get; set; } = "<color={color}><b>TPS:</b> {tps}/{maxTps}</color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string TpsVisual { get; set; } = "BOTH";
        public int TpsXCordinate { get; set; } = -60;
        public int TpsYCordinate { get; set; } = 20;
        public int TpsFontSize { get; set; } = 25;

        [Description("ROUND TIME Settings:")]
        public bool EnableRoundTime { get; set; } = true;
        public string RoundTime { get; set; } = "<color={color}><b>Round Time:</b> {round_time}</color>";
        [Description("GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay")]
        public string RoundTimeVisual { get; set; } = "BOTH";
        public int RoundTimeXCordinate { get; set; } = 400;
        public int RoundTimeYCordinate { get; set; } = 20;
        public int RoundTimeFontSize { get; set; } = 25;

        [Description("Player HUD Settings:")]
        public bool EnablePlayerHud { get; set; } = true;
        [Description("You can use {displayname} instead of {nickname}")]
        public string PlayerHud { get; set; } = "<color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public int PlayerHudFontSize { get; set; } = 33;
        
        [Description("Spectator List:")]
        public bool EnableSpectatorList { get; set; } = true;
        public string SpectatorListHeader { get; set; } = "<color={color}>👥 Spectators ({count})</color>";
        public string SpectatorListPlayers { get; set; } = "<color={color}>• {nickname}</color>";
        public List<RoleTypeId> HiddenForRoles { get; set; } = [RoleTypeId.Overwatch];
        public int SpectatorListYCordinate { get; set; } = 100;
        public int SpectatorListFontSize { get; set; } = 28;

        [Description("Ammo Counter:")]
        public bool EnableAmmoCounter { get; set; } = true;
        public string WeaponName { get; set; } = "<space=-900><color={color}>{weapon}</color> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt";
        public string AmmoCounter { get; set; } = "<space=-900><b><color={color}>{current} / {max}</color></b> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt";
        public int AmmoCounterYCordinate { get; set; } = 950;
        public int AmmoCounterFontSize { get; set; } = 28;

        [Description("Spectator HUD Settings:")]
        public bool EnableSpectatorHud { get; set; } = true;
        [Description("You can use {displayname} instead of {nickname}")]
        public string SpectatorHud { get; set; } = "<color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public int SpectatorHudFontSize { get; set; } = 33;
        [Description("If true, will not show the spectated player's nickname when they are a skeleton (SCP-3114). This mimics the behavior of the base-game spectator UI.")]
        public bool HideSkeletonNickname { get; set; } = true;

        [Description("Spectator Map Info:")]
        public bool EnableSpectatorMapInfo { get; set; } = true;
        public string SpectatorMapInfo { get; set; } = "<b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color>";
        public int MapInfoXCordinate { get; set; } = 650;
        public int MapInfoYCordinate { get; set; } = 1000;
        public int MapInfoFontSize { get; set; } = 27;

        [Description("Spectator Server Info:")]
        public bool EnableSpectatorServerInfo { get; set; } = true;
        public string SpectatorServerInfo { get; set; } = "<b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color>";
        public int ServerInfoXCordinate { get; set; } = -500;
        public int ServerInfoYCordinate { get; set; } = 1000;
        public int ServerInfoFontSize { get; set; } = 27;
    }
}