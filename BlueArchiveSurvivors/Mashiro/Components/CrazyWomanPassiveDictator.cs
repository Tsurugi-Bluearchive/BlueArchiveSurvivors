using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Mashiro.Components
{
    internal class CrazyWomanPassiveDictator : MonoBehaviour
    {
        public enum MashiroPassive
        {
            None,
            BloodSewing,
            BloodLust
        }

        public SkillDef BloodSewing;
        public SkillDef BloodLust;
        public GenericSkill PassiveSlot;

        public MashiroPassive GetPassiveType()
        {
            if ((bool)PassiveSlot)
            {
                if (PassiveSlot.skillDef == BloodSewing)
                {
                    return MashiroPassive.BloodSewing;
                }
                else if (PassiveSlot.skillDef == BloodLust)
                {
                    return MashiroPassive.BloodLust;
                }
                return MashiroPassive.None;
            }
            return MashiroPassive.None;
        }

    }
}
