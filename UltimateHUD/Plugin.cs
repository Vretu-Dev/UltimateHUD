using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace UltimateHUD
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "UltimateHUD";
        public override string Author => "Vretu";
        public override string Prefix => "UltimateHud";
        public override Version Version => new Version(3, 2, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);
        public static Plugin Instance { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers.RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            EventHandlers.UnregisterEvents();
            base.OnDisabled();
        }
    }
}