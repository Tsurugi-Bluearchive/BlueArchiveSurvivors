using BAMod.GlobalContent.Components;
using R2API;
using RoR2;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.GlobalContent.Scripts
{
    static class SimBulletConnectionManager
    {
        private static bool initialized;

        public static GameObject SimBulletServerObject;
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
            while (!NetworkServer.active && !NetworkClient.active)
            {
                await Task.Yield();
            }

            if (NetworkServer.active)
            {
                SimBulletManager.Init();
            }
        }
    }
}