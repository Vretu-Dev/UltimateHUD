using Exiled.API.Interfaces;
using HintServiceMeow.Core.Enum;
using System.ComponentModel;

namespace UltimateHUD
{
    public class Config : IConfig
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
    }
}