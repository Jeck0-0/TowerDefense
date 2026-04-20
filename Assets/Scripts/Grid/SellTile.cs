namespace TowerDefense
{
    public class SellTile : Tile
    {
        public override bool CanPlace(Tower t)
        {
            return true;
        }

        public override void PlaceTower(Tower t)
        {
            Destroy(t.gameObject);
        }

        public override void RemoveTower()
        {
            
        }
    }
}