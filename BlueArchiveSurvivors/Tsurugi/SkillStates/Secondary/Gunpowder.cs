using BAMod.Tsurugi;
using BAMod.Tsurugi.Content;
using BAMod.Tsurugi.SkillStates.BaseStates;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BAMod.Tsurugi.SkillStates.Secondary
{
    internal class Gunpowder : BaseTsurugiSkillState
    {
        protected override float baseDuration => 4;
        protected override float baseFireDelay => 0.5f;
        protected override float fireTime => 1;
        private bool fired = false;
        public GameObject hitEffectPrefab = FireBarrage.hitEffectPrefab;
        public GameObject tracerEffectPrefab = FireBarrage.tracerEffectPrefab;
        public DamageTypeCombo damageType = DamageType.Generic;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge > fireDelay && IsKeyDownAuthority() && !fired)
                {
                    var aimRay = GetAimRay();
                    if (!TsurugiMain.secondaryMysterious)
                    {
                        var pelletVectors = ScatterVectors(aimRay.direction, 30, 20f, 0.2f);
                        foreach (var p in pelletVectors)
                        {
                            BulletAttack bullet = new BulletAttack
                            {
                                owner = base.gameObject,
                                weapon = base.gameObject,
                                origin = aimRay.origin,
                                aimVector = p + aimRay.direction * 2f,
                                minSpread = 0f,
                                maxSpread = base.characterBody.spreadBloomAngle,
                                bulletCount = 1U,
                                procCoefficient = 1f,
                                damage = base.characterBody.damage * TsurugiStaticValues.justiceDamage,
                                force = 3,
                                falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                                tracerEffectPrefab = this.tracerEffectPrefab,
                                hitEffectPrefab = this.hitEffectPrefab,
                                isCrit = base.RollCrit(),
                                HitEffectNormal = false,
                                stopperMask = LayerIndex.world.collisionMask,
                                smartCollision = true,
                                maxDistance = 300f,
                                damageType = damageType,
                                radius = 4
                            };
                            DamageAPI.AddModdedDamageType(bullet, TsurugiCustomDamageTypes.GunpowderHeal);
                            bullet.Fire();
                        }
                    }
                    else
                    {
                        var mysteriousSearch = new BullseyeSearch();
                        mysteriousSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
                        mysteriousSearch.searchDirection = aimRay.direction;
                        mysteriousSearch.searchOrigin = aimRay.origin;
                        mysteriousSearch.maxAngleFilter = 60;
                        mysteriousSearch.maxDistanceFilter = 100;
                        mysteriousSearch.viewer = characterBody;
                        mysteriousSearch.RefreshCandidates();
                        foreach (var target in mysteriousSearch.candidatesEnumerable)
                        {
                            var mysteriousKaboom = new BlastAttack()
                            {
                                attacker = this.gameObject,
                                radius = 5,
                                canRejectForce = true,
                                baseForce = 0,
                                crit = base.RollCrit(),
                                teamIndex = this.characterBody.teamComponent.teamIndex,
                                falloffModel = BlastAttack.FalloffModel.None,
                                position = target.position,
                                baseDamage = 10 * damageStat,
                                procCoefficient = 5f
                            };
                            mysteriousKaboom.AddModdedDamageType(TsurugiCustomDamageTypes.GunpowderHeal);
                            mysteriousKaboom.Fire();
                        }
                    }

                    AddRecoil(20, 20, 0, 0);
                    fired = true;
                }
                if (fired && fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                else if (!fired && !IsKeyDownAuthority() && fixedAge < fireDelay)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!fired)
            {
                activatorSkillSlot.AddOneStock();
            }
            else if (stock <= 0)
            {
                TsurugiMain.confirmedSecondaryKills = 0;
                TsurugiMain.secondaryMysterious = false;
                skillLocator.secondary.SetSkillOverride(this.gameObject, TsurugiSurvivor.GunpowderReload, GenericSkill.SkillOverridePriority.Default);
            }
        }

        public static List<Vector3> ScatterVectors(
            Vector3 forward,
            int pelletCount,
            float maxAngle,
            float randomness = 0.5f
        )
        {
            List<Vector3> directions = new List<Vector3>();

            forward.Normalize();
            Quaternion baseRotation = Quaternion.LookRotation(forward);

            for (int i = 0; i < pelletCount; i++)
            {
                float t = (i + Random.value * randomness) / pelletCount;
                float spreadAngle = maxAngle * Mathf.Sqrt(t);

                float theta = (i * 137.5f) % 360f;
                theta += Random.Range(-180f, 180f) * randomness;

                Quaternion aroundForward = Quaternion.AngleAxis(theta, Vector3.forward);
                Quaternion outwardTilt = Quaternion.AngleAxis(spreadAngle, Vector3.right);

                Vector3 dir = baseRotation * (aroundForward * outwardTilt * Vector3.forward);

                dir = Vector3.Slerp(forward, dir, 0.9f);

                directions.Add(dir.normalized);
            }

            return directions;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
