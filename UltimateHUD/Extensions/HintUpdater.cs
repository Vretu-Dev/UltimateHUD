using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;
using MEC;

namespace UltimateHUD.Extensions
{
    public static class HintUpdater
    {
        private static CoroutineHandle hintCoroutine;
        private static Config Config => Plugin.Instance.Config;

        public static void RegisterUpdater()
        {
            if (Config.EnableClock || Config.EnableTps || Config.EnableRoundTime)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            }
        }

        public static void UnregisterUpdater()
        {
            if (Config.EnableClock || Config.EnableTps || Config.EnableRoundTime)
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            }
        }

        private static void OnRoundStarted() => Start();
        private static void OnRoundEnded(RoundEndedEventArgs ev) => Stop();

        private static void Start()
        {
            Stop();
            hintCoroutine = Timing.RunCoroutine(Updater());
        }

        private static void Stop()
        {
            if (hintCoroutine.IsRunning)
                Timing.KillCoroutines(hintCoroutine);
        }

        private static IEnumerator<float> Updater()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    Hints.RefreshClock(player.ReferenceHub);
                    Hints.RefreshTps(player.ReferenceHub);
                    Hints.RefreshRoundTime(player.ReferenceHub);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
