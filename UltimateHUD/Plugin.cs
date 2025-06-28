using System;
using LabApi.Features;
using LabApi.Loader;
using Loader = LabApi.Loader.Features.Plugins.Plugin;

namespace UltimateHUD
{
    public class Plugin : Loader
    {
        public override string Name => "UltimateHUD";
        public override string Description => "Customizable HUD for SCP:SL.";
        public override string Author => "Vretu";
        public override Version Version => new Version(5, 7, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public static Plugin Instance { get; private set; }
        public Translations Translation { get; private set; }
        public Config Config { get; private set; }

        public override void Enable()
        {
            Instance = this;
            Hints.RegisterHints();
            EventHandlers.RegisterEvents();
        }

        public override void Disable()
        {
            Instance = null;
            Hints.UnregisterHints();
            EventHandlers.UnregisterEvents();
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config config);
            Config = config ?? new Config();
            this.TryLoadConfig("translation.yml", out Translations translation);
            Translation = translation ?? new Translations();
        }
    }
}