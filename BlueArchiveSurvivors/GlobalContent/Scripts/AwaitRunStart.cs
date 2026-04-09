using RoR2;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.GlobalContent.Scripts
{
    static class AwaitRunStart
    {
        private static bool initialized;

        public static void Init()
        {
            On.RoR2.Run.Start += Run_Start;
        }

        private static void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            StartBehaviorsAsync();
        }

        public static async Task StartBehaviorsAsync()
        {
            initialized = true;

            while (!NetworkServer.active)
            {
                await Task.Yield();
            }

            SimBulletManager.Init();
        }
    }
}