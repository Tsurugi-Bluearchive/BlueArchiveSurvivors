using System.Collections.Generic;
using UnityEngine;
using static BAMod.GlobalContent.Scripts.SimBulletManager;

namespace BAMod.GlobalContent.Scripts
{
    internal class ServerSimBulletPool
    {
        public Dictionary<int, SimBullet> ServerBullets = new Dictionary<int, SimBullet>();
        public List<(int key, RaycastHit hit)> PendingDestroy = new List<(int key, RaycastHit hit)>();

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

        /// <summary>
        /// Call this at the start of every ServerPhysicsUpdate.
        /// </summary>
        public void ProcessPendingDestroy()
        {
            if (PendingDestroy.Count == 0) return;

            foreach (var entry in PendingDestroy)
            {
                if (ServerBullets.TryGetValue(entry.key, out var bullet))
                {
                    bullet.active = false;
                }
                ServerBullets.Remove(entry.key);
            }

            PendingDestroy.Clear();
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