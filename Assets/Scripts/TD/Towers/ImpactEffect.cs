using UnityEngine;

namespace TowerDefense
{
    public class ImpactEffect : MonoBehaviour
    {
        public LineRenderer lr;
        public GameObject fx;

        public void SetImpact(float scale, Color? color)
        {
            fx.transform.localScale = new Vector3(scale, scale, 1);
            if (color.HasValue)
            {
                foreach (var r in fx.GetComponentsInChildren<SpriteRenderer>())
                    r.color = color.Value;
            }
        }
        
        public void SetRange(float range, Color? color = null)
        {
            if (range <= 0.001f) return;
            if (lr == null)
            {
                Debug.LogWarning("No LineRenderer set for ImpactEffect", gameObject);
                return;
            }
            
            lr.gameObject.SetActive(true);
            lr.transform.localScale = Vector3.one * range;

            if (color != null)
            {
                var gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new(color!.Value, 0.0f) },
                    new GradientAlphaKey[] { new(color!.Value.a, 0.0f) });
                lr.colorGradient = gradient;
            }
        }
    
    }
    
}
