using BAMod.GlobalContent.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.GlobalContent.Interfaces
{
    internal interface ISimBulletCharacterUser
    {
        public void FireSimBullet(SimBulletManager.SimBullet Bullet, int index);
        [Command]
        public void CmdFireSimBullet(SimBulletManager.SimBullet Bullet, int index);
        [ClientRpc]
        public void RpcInstantiateSimBullet(out GameObject bullet, GameObject bulletin, SimBulletManager.SimBullet bulletinformation);
    }
}
