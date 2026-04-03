using EntityStates;
using RoR2;
using RoR2.Skills;
using RoR2BepInExPack.GameAssetPaths;
using UltrakillMod.V1.SkillStates.BaseStates;
using UnityEngine.PlayerLoop;

namespace BA.Tsurugi.SkillStates.BaseStates
{
    public abstract class BaseTsurugiSkillState : BaseSkillState
    {
        /// <summary>
        /// Cached SchoolgirlCharacterMain
        /// </summary>
        public TsurugiCharacterMain TsurugiMain;

        /// <summary>
        /// Base fire delay
        /// </summary>
        protected abstract float baseFireDelay {get;}

        /// <summary>
        /// Modified fire delay to scale with attack speed
        /// </summary>
        /// <returns>baseFireDelay * attackSpeedStat</returns>
        public float fireDelay => baseFireDelay * attackSpeedStat;

        /// <summary>
        /// The base duration
        /// </summary>
        protected abstract float baseDuration {get;}

        /// <summary>
        /// The base duration modified for attack speed
        /// </summary>
        /// <returns>baseDuration * attackSpeedStat</returns>
        public float duration => baseDuration * attackSpeedStat;

        /// <summary>
        /// Inverted duration that gets longer with attack speed
        /// </summary>
        /// <returns>baseDuration / attackSpeedStat</returns>
        public float invduration => baseDuration / attackSpeedStat;

        /// <summary>
        /// Get; Set; the stock of the skill based on activatorSKillSlot.stock
        /// </summary>
        /// <returns>The appropriate skill stock</returns>
        public int stock
        {
            get { return activatorSkillSlot.stock; }
            set { activatorSkillSlot.stock = value;}
        }

        /// <summary>
        /// Bool responsible for checking if fully channeled to completion
        /// </summary>
        /// <returns>fixedAge > duration AND (isKeyDown OR keyJustReleased)</returns>
        public bool FullyChanneled {
            get
            {
                if (fixedAge > duration && (IsKeyDownAuthority() || IsKeyJustReleasedAuthority()))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// The unmodified fire time
        /// </summary>
        /// <returns>5f</returns>
        protected abstract float fireTime { get; }

        /// <summary>
        /// Called to check channel state
        /// </summary>
        /// <returns>if currnetly channeling</returns>
        public bool Channeling
        {
            get
            {
                if (FullyChanneled)
                    return false;
                if (IsKeyDownAuthority())
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Charge Condition for charging skills
        /// </summary>
        /// <returns>if the skill can be fired off (true) or still charging (false)</returns>
        public bool Charged
        {
            get
            {
                if (IsKeyJustReleasedAuthority() && fixedAge > fireDelay)
                    return true;
                if (FullyChanneled)
                    return true;
                return false;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            TsurugiMain = characterBody.gameObject.GetComponent<TsurugiCharacterMain>();
        }
    }
}
