using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAMod.Arisu.SkillStates.SpecialLock
{
    internal class SpecialLockDown : BaseSkillState
    {
        public override void FixedUpdate()
        {
            outer.SetNextStateToMain();
            return;
        }
    }
}
