using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Tsurugi.Components
{
    internal class CrazyWomanPassiveDictator : MonoBehaviour
    {
        public enum TsurugiPassive
        {
            None,
            BloodSewing,
            BloodLust
        }

        public SkillDef BloodSewing;
        public SkillDef BloodLust;
        public GenericSkill PassiveSlot;

        public TsurugiPassive GetPassiveType()
        {
            if ((bool)PassiveSlot)
            {
                if (PassiveSlot.skillDef == BloodSewing)
                {
                    return TsurugiPassive.BloodSewing;
                }
                else if (PassiveSlot.skillDef == BloodLust)
                {
                    return TsurugiPassive.BloodLust;
                }
                return TsurugiPassive.None;
            }
            return TsurugiPassive.None;
        }

    }
}
