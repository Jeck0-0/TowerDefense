using Sirenix.OdinInspector;
using UnityEngine;

namespace TowerDefense
{
    public class BitDrop : MonoBehaviour
    {
        private int bits;
        
        public void Initialize(int bits)
        {
            this.bits = bits;
        }

        public void Collect()
        {
            GameStats.Instance.ModifyCoins(bits);
            Destroy(gameObject);
        }
    }
}