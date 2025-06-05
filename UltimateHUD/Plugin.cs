using System;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace UltimateHUD
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "UltimateHUD";
        public override string Description => "Customizable HUD for SCP:SL.";
        public override string Author => "Vretu";
        public override Version Version => new Version(5, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public static Plugin Instance { get; private set; }

        public override void Enable()
        {
            Instance = this;
            EventHandlers.RegisterEvents();
        }

        public override void Disable()
        {
            Instance = null;
            EventHandlers.UnregisterEvents();
        }
    }
}