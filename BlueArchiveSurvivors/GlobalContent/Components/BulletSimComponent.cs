using BAMod.GlobalContent.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

namespace BAMod.GlobalContent.Components
{
    internal class BulletSimComponent : NetworkBehaviour
    {
        public SimBulletType type;
        public bool Destroy;
        public int ID;
        void Awake()
        {
            if (!isClient)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (isClient)
            {
                ClientVisualUpdate();
            }
        }

        void OnDestroy()
        {
            if (!isClient) return;
            
        }

        [ClientCallback]
        void ClientVisualUpdate()
        {

        }
    }
}
