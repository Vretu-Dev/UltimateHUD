using Exiled.API.Interfaces;

namespace UltimateHUD
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public int MsRefreshRate { get; set; } = 500;
        public string Clock { get; set; } = "<color={color}><b>Time:</b> {time}</color>";
        public string Tps { get; set; } = "<color={color}><b>TPS:</b> {tps}/{maxTps}</color>";
        public string PlayerHud { get; set; } = "<color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public string SpectatorHud { get; set; } = "<color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>";
        public string SpectatorInfo { get; set; } = "<b>Generators:</b> <color=orange>{engaged}</color>/<color=orange>{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color>";
    }
}