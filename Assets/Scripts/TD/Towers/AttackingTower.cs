using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefense
{
    public abstract class AttackingTower : Tower
    {
        [Header("Advanced")]
        public Transform cannon;
        public float attackAngleThreshold = 5;
        public bool rotateTowardsTarget = true;
    
        public Stat RotationSpeed;
        public Stat Damage;
        public Stat AttackSpeed;
    
        [BoxGroup("Sound")]
        [Range(0f, 1f)]
        public float attackSoundVolume = 0.6f;
    
        [Header("Runtime")]
        public Targetable target;
    
        public delegate void AttackEvent(AttackingTower attacker, Targetable primaryTarget, IEnumerable<Targetable> allTargets);
        public event AttackEvent OnAttack;
        /*public delegate void ShootEvent(AttackingTower attacker, Targetable primaryTarget, IEnumerable<Targetable> allTargets);
        public event ShootEvent OnShoot;*/
        
        public delegate float ModifyDamage(AttackingTower attacker, GameObject attackSource, ref float currentDamage, Targetable target);
        public event ModifyDamage OnModifyDamage;

        /*public delegate void TargetHit(AttackingTower attacker, Targetable primary, IEnumerable<Targetable> allTargets);
        public event TargetHit OnTargetHit;*/
        
        public delegate void OnDamage(AttackingTower attacker, Targetable target, float originalAmount, float unblockedAmount, bool killed);
        public event OnDamage OnDamageEvent;
        public delegate void OnTargetLost(AttackingTower attacker, Targetable previousTarget);
        public event OnTargetLost OnTargetLostEvent;
        public delegate void OnTargetFound(AttackingTower attacker, Targetable target);
        public event OnTargetFound OnTargetFoundEvent;


        public void DamageTarget(Targetable target, float damage, GameObject attackSource)
        {
            //damage = OnModifyDamage?.Invoke(this, attackSource, ref damage, target) ?? damage;
            OnModifyDamage?.Invoke(this, attackSource, ref damage, target);
            var dmgReport = target.Damage(damage);
            OnDamageEvent?.Invoke(this, target, damage, dmgReport.Dealt, dmgReport.Killed);
        }
        
        public override Stats GetStats()
        {
            if(stats != null) 
                return stats;
    
            var tempStats = base.GetStats();
            tempStats.AddStat("rotationSpeed", RotationSpeed);
            tempStats.AddStat("damage", Damage);
            tempStats.AddStat("attackSpeed", AttackSpeed);
            return tempStats;
        }
    
    
        public override void ManagedUpdate()
        {
            nextShotTime -= Time.deltaTime;
            UpdateAttack();
        }
    
        private float nextShotTime;
        private bool hadTargetLastFrame;
        protected virtual void UpdateAttack()
        {
            if (!IsValidTarget(target))
            {
                if (hadTargetLastFrame)
                    OnTargetLostEvent?.Invoke(this, target);
                hadTargetLastFrame = false;
    
                FindNewTarget();
    
                if (target == null)
                    return;
    
                OnTargetFoundEvent?.Invoke(this, target);
                hadTargetLastFrame = true;
            }
    
            if(rotateTowardsTarget)
                RotateTowards(target.transform.position);
    
            if (!FacingTarget()) return;
    
            if (nextShotTime > 0) return;
            nextShotTime = 1 / AttackSpeed;
    
            OnAttack?.Invoke(this, target, null);
            Attack();
            
            var pitch = new AudioParams.Pitch(AudioParams.Pitch.Variation.Medium);
            var repetition = new AudioParams.Repetition(.05f);
            AudioController.Instance.PlaySound2D($"tower_{towerID}_shoot", attackSoundVolume, pitch: pitch, repetition: repetition);
        }
    
        protected abstract void Attack();
        
        //protected abstract void Shoot(Targetable target, Vector3? direction);
        
    
        protected virtual bool IsValidTarget(Targetable t)
        {
            if (t == null) return false;
            if (!t.isAlive) return false;
    
            float distance = Vector3.Distance(t.transform.position, transform.position);
            if (distance > MaxRange || distance < MinRange)
                return false;
    
            return true;
        }
    
        protected bool FacingTarget()
        {
            if (attackAngleThreshold >= 360) 
                return true;
            Vector3 directionToTarget = target.transform.position - cannon.position;
            float angleToTarget = Vector3.Angle(cannon.transform.up, directionToTarget);
            return angleToTarget < attackAngleThreshold;
        }
    
        protected virtual void RotateTowards(Vector3 target)
        {
            Vector3 direction = target - cannon.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
    
            cannon.rotation = Quaternion.Slerp(cannon.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    
        protected virtual bool FindNewTarget()
        {
            Enemy bestEnemy = null;
            float bestDistance = float.MaxValue;

            foreach (var e in GetEnemiesInRange())
            {
                if (!IsValidTarget(e))
                    continue;
    
                float enemyDist = e.movement.DistanceFromTarget();
                if (enemyDist < bestDistance)
                {
                    bestDistance = enemyDist;
                    bestEnemy = e;
                }
            }
            target = bestEnemy;
            return target != null;
        }

        protected List<Enemy> GetEnemiesInRange()
        {
            var hit = Physics2D.CircleCastAll(transform.position, MaxRange, 
                Vector2.zero, 0, layerMask: LayerMask.GetMask("Targetable"));

            hit.Where(x => Vector3.Distance(x.collider.transform.position, transform.position) >= MinRange);
            return hit.Select(x => x.collider.GetComponent<Enemy>()).Where(x => x != null && x.isAlive).ToList();
        }
    
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, MaxRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MinRange);
        }
    }
}
