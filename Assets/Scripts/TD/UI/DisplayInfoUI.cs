using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;


namespace TowerDefense
{
    public class DisplayInfoUI : Singleton<DisplayInfoUI>
    {
        [SerializeField] GameObject parent;
    
        [SerializeField] Image iconImage;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;
    
        [SerializeField] GameObject shopParent;
        [SerializeField] List<StatInfoUI> statList;


        private void Awake()
        {
            Hide();
        }


        BaseDisplayInfo infoBeforePreview;
        public void Preview(BaseDisplayInfo info)
        {
            ShowInfo(info);
        }
        public void StopPreview()
        {
            ShowInfo(infoBeforePreview);
        }

        public void Show(BaseDisplayInfo info)
        {
            if (info == null)
                Hide();

            infoBeforePreview = info;

            shopParent.SetActive(false);
            statList.ForEach(x => x.gameObject.SetActive(false));

            ShowInfo(info);
    
            if (info is TowerDisplayInfo)
            {
                ShowTower(info as TowerDisplayInfo);
                return;
            }
    
            if (info is ShopDisplayInfo) 
            {
                ShowShop(info as ShopDisplayInfo);
            }
        }
    
        public void UpdateInfo()
        {
            Show(TileSelectionManager.Instance?.SelectedTile?.GetDisplayInfo());
        }
    
    
        protected void ShowShop(ShopDisplayInfo info)
        {
            shopParent?.SetActive(true);
        }
    
        protected void ShowTower(TowerDisplayInfo info)
        {
            var stats = info.tower.GetStats();
    
            if (stats != null)
            {
                int i = 0;
                foreach (var stat in stats.stats)
                {
                    if (!StatDisplayOptions.ShouldShowStat(stat.Key) || stat.Value.MaxValue == 0)
                        continue;
    
                    //statList[i].DisplayStat(stat.Value, info.tower.upgradeHandler);
    
                    i++;
                    if (i >= statList.Count)
                        break;
                }
            }

        }
    
        protected void ShowInfo(BaseDisplayInfo info)
        {
            parent.SetActive(true);
            iconImage.sprite = info.icon;
            titleText.text = info.title;
            descriptionText.text = info.description;
        }
    
        public void Hide()
        {
            parent.SetActive(false);
        }
    
    }
    
    public class BaseDisplayInfo
    {
        public string title;
        public string description;
        public Sprite icon;
        public BaseDisplayInfo() { }
        public BaseDisplayInfo (string title, string description, Sprite icon)
        {
            this.title = title;
            this.description = description;
            this.icon = icon;
        }
    }
    
    public class ShopDisplayInfo : BaseDisplayInfo
    {
        public ShopDisplayInfo() { }
        public ShopDisplayInfo(string title, string description, Sprite icon) : base(title, description, icon) { }
    }
    
    public class TowerDisplayInfo : BaseDisplayInfo 
    {
        public Tower tower;
    
        public TowerDisplayInfo() { }
        public TowerDisplayInfo(string title, string description, Sprite icon, Tower tower) : base(title, description, icon) { this.tower = tower; }
    }
}
