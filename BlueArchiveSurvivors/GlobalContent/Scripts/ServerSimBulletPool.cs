using System.Collections.Generic;
using UnityEngine;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Scripts
{
    internal class ServerSimBulletPool
    {
        public Dictionary<int, SimBullet> ServerBullets = new Dictionary<int, SimBullet>();
        public Queue<(int key, RaycastHit hit)> PendingDestroy = new Queue<(int key, RaycastHit hit)>();

        private static int _nextBulletId = 1;

        /// <summary>
        /// Tries to reuse an inactive bullet from the pool.
        /// </summary>
        public bool TryReuseBullet(out int id, SimBullet newBullet)
        {
            id = -1;

            foreach (var pair in ServerBullets)
            {
                if (pair.Value != null && !pair.Value.active)
                {
                    id = pair.Key;
                    ServerBullets[id] = newBullet;
                    newBullet.active = true;
                    return true;
                }
            }
            return false;
        }

        public void ProcessPendingDestroy()
        {
            if (PendingDestroy.Count == 0) return;

            var destroySnapshot = new List<(int key, RaycastHit hit)>(PendingDestroy);

            PendingDestroy.Clear();

            foreach (var entry in destroySnapshot)
            {
                ServerBullets.Remove(entry.key);
            }
        }

        /// <summary>
        /// Creates a new bullet with a unique ID.
        /// </summary>
        public int CreateBullet(SimBullet newBullet)
        {
            int id = _nextBulletId++;
            newBullet.active = true;
            ServerBullets[id] = newBullet;
            return id;
        }

        public void Modify(Dictionary<int, SimBullet> modification)
        {
            foreach (var pair in modification)
            {
                ServerBullets[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Immediately removes a bullet from the pool.
        /// </summary>
        public void RemoveBullet(int id)
        {
            if (ServerBullets.TryGetValue(id, out var bullet))
            {
                bullet.active = false;
            }
            ServerBullets.Remove(id);
        }

        /// <summary>
        /// Clears everything (useful on scene load or mod unload).
        /// </summary>
        public void ClearAll()
        {
            ServerBullets.Clear();
            PendingDestroy.Clear();
            _nextBulletId = 1;
        }
    }
}