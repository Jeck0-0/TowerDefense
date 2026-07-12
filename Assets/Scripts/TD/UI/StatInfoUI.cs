using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TowerDefense
{
    public class StatInfoUI : MonoBehaviour
    {
        [SerializeField] TMP_Text statName;
        [SerializeField] TMP_Text statValue;
        [SerializeField] Image currentValue;
        [SerializeField] Image upgradeValue;
    
        /*
        Stat stat;
        UpgradeHandler upgradeHandler;
        float maxValue;

        public void DisplayStat(Stat stat, UpgradeHandler upgradeHandler = null)
        {
            if(this.stat != null)
                this.stat.OnValueChanged -= UpdateValue;
            if(this.upgradeHandler != null)
                this.upgradeHandler.OnLevelChanged -= OnLevelUp;

            this.maxValue = Stat.GetValueWithModifiers(stat, upgradeHandler.GetAllUpgradesForStat(stat.Name));
            if (maxValue <= 0)
                return;

            this.upgradeHandler = upgradeHandler;
            this.stat = stat;
            
            stat.OnValueChanged += UpdateValue;
            upgradeHandler.OnLevelChanged += OnLevelUp;

            gameObject.SetActive(true);
            UpdateValue(null);
        }
    
        private void OnLevelUp()
            => UpdateValue(null);

        public void UpdateValue(object args)
        {
            var mod = upgradeHandler.GetNextUpgrade()?.statUpgrades?.Find(x=>x.stat == stat.Name)?.modifier;
            
            statName.text = StatDisplayOptions.GetInfo(stat.Name).name;
            statValue.text = stat.Value.ToString();
    
            float currentProgress = stat / maxValue;
    
            currentValue.transform.localScale = new Vector3(currentProgress, 1, 1);
            currentValue.color = StatDisplayOptions.GetInfo(stat.Name).color;
    
            if (mod != null)
            {
                var mods = stat.modifiers.Values.ToList();
                mods.Add(mod);
                float upgradedValue = Stat.GetValueWithModifiers(stat, mods);
                float upgradeProgress = upgradedValue / maxValue;
                
                upgradeValue.transform.localScale = new Vector3 (upgradeProgress, 1, 1);
            }
            else
            {
                upgradeValue.transform.localScale = Vector3.zero;
            }
        }*/
    }
    
}
