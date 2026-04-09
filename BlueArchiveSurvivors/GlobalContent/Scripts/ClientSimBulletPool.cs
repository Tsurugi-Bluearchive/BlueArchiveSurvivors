using BAMod.GlobalContent.Components;
using System.Collections.Generic;
using UnityEngine;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Scripts
{
    static class ClientSimBulletPool
    {
        public static Dictionary<int, (BulletSimComponent bulletInstance, SimBullet bullet)> ClientBullets = new();
        public static Queue<int> DestroyClientBullets = new();

        private static int _nextClientId = 1;

        public static bool TryGetBullet(int id, out BulletSimComponent component, out SimBullet simBullet)
        {
            component = null;
            simBullet = null;
            if (ClientBullets.TryGetValue(id, out var pair))
            {
                component = pair.bulletInstance;
                simBullet = pair.bullet;
                return true;
            }
            return false;
        }

        public static void AddBullet(int id, BulletSimComponent component, SimBullet simBullet)
        {
            if (component != null)
            {
                component.ID = id;
            }
            ClientBullets[id] = (component, simBullet);
        }

        /// <summary>
        /// Attempts to reuse an existing inactive bullet of the same prefab type.
        /// Returns true if a free instance was found and assigned.
        /// </summary>
        public static bool TryReuseBullet(SimBullet newBulletData, BulletSimComponent prefabReference, out int id, out BulletSimComponent reusedComponent)
        {
            id = -1;
            reusedComponent = null;

            foreach (var entry in ClientBullets)
            {

                var pair = entry.Value;
                if (pair.bullet.active ||
                    pair.bulletInstance == null ||
                    pair.bulletInstance.PrefabID != prefabReference.PrefabID)
                {
                    continue;
                }

                id = entry.Key;
                reusedComponent = pair.bulletInstance;

                reusedComponent.ID = id;
                ClientBullets[id] = (reusedComponent, newBulletData);

                return true;
            }

            return false;
        }

        public static void UpdatePool()
        {
            foreach(var queued in DestroyClientBullets)
            {
                ClientBullets[queued].bulletInstance.shouldDestroy = true;
                DestroyClientBullets.Dequeue();      }
        }
        public static void RemoveBullet(int id)
        {
            if (ClientBullets.TryGetValue(id, out var pair))
            {
                if (pair.bulletInstance != null)
                {
                    pair.bulletInstance.shouldDestroy = true;
                }
            }
            ClientBullets.Remove(id);
        }

        public static void ClearAll()
        {
            foreach (var pair in ClientBullets.Values)
            {
                if (pair.bulletInstance != null)
                {
                    pair.bulletInstance.shouldDestroy = true;
                }
            }
            ClientBullets.Clear();
        }

        public static int GetNextId()
        {
            return _nextClientId++;
        }
    }
}