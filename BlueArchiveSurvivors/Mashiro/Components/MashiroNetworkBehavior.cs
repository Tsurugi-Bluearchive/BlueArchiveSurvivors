using BAMod.GlobalContent.Components;
using BAMod.GlobalContent.Interfaces;
using BAMod.GlobalContent.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BAMod.Mashiro.Components
{
    internal class MashiroNetworkBehavior : NetworkBehaviour, ISimBulletCharacterUser
    {
        public void FireSimBullet(SimBulletManager.SimBullet bullet, int index)
        {
            if (hasAuthority)
            {
                CmdFireSimBullet(bullet, index);
            }
        }

        [Command]
        public void CmdFireSimBullet(SimBulletManager.SimBullet bullet, int index)
        {
            if (SimBulletManager.ReturnSimbBulletObject(out var bulletInformation, index))
            {
                RpcInstantiateSimBullet(out bulletInformation, bulletInformation, bullet);
                NetworkServer.Spawn(bulletInformation);
            }
        }

        [ClientRpc]
        public void RpcInstantiateSimBullet(out GameObject bullet, GameObject bulletin, SimBulletManager.SimBullet bulletinformation)
        {
            bullet = Instantiate(bulletin);
            bullet.GetComponent<SimBulletNetworkBehavior>().SimBullet = bulletinformation;
        }
    }
}