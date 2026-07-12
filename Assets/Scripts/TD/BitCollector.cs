using System;
using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BitCollector : MonoBehaviour, IStatObject
    {
        public Stat CollectRange;

        public AnimationCurve speedByDistance;
        public float moveSpeed;
        public float lerp;
        
        private Stats stats;
        
        public Stats GetStats()
        {
            if(stats != null) 
                return stats;
    
            stats = new Stats();
            stats.AddStat("collectRange", CollectRange);
            return stats;
        }

        private void Awake()
        {
            CollectRange.OnValueChanged += (Stat.StatValueChangedEventArgs args) =>
            {
                GetComponent<CircleCollider2D>().radius = args.newValue;
            };
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Collectable"))
            {
                float dist = Vector3.Distance(transform.position.XY(), other.transform.position.XY());
                float moveBy = speedByDistance.Evaluate(dist / CollectRange);
                
                bool reached = other.transform.MoveTowards(transform.position, 
                    Time.deltaTime * moveBy, rotate: false);
                
                if (reached)
                {
                    var drop = other.transform.GetComponent<BitDrop>();
                    drop.Collect();
                }
            }
        }
    }
}