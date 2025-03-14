using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;


namespace TowerDefense
{
    public class ProgressBarUI : ManagedBehaviour
    {
        [SerializeField]
        private float barFillSpeed = 1f;
    
        [SerializeField]
        private Transform bar = default;
    
        [SerializeField]
        private Transform barEdge = default;
    
        [SerializeField]
        private float fullBarScale = default;
    
        [SerializeField]
        private AnimationCurve fillAnimationCurve = default;
    
        protected float CurrentProgressNormalized { get; private set; }
    
        public Vector2 GetBarEdgePos()
        {
            return barEdge.position;
        }
    
        public void ShowProgress(float progressNormalized, bool immediate = false, System.Action fillCompleteCallback = null)
        {
            if (immediate || !gameObject.activeInHierarchy)
            {
                Tween.Cancel(bar.transform.GetInstanceID());
                bar.transform.localScale = GetBarScaleForProgress(progressNormalized);
                fillCompleteCallback?.Invoke();
            }
            else
            {
                //bar.transform.localScale = GetBarScaleForProgress(CurrentProgressNormalized);
                //float fillDuration = Mathf.Abs((progressNormalized - CurrentProgressNormalized) / barFillSpeed);
                float fillDuration = Mathf.Abs((progressNormalized - bar.transform.localScale.x) / barFillSpeed);
                OnFillStart();
                Tween.LocalScale(bar.transform, GetBarScaleForProgress(progressNormalized), fillDuration, 0f, fillAnimationCurve, completeCallback: () =>
                {
                    fillCompleteCallback?.Invoke();
                    OnFillEnd();
                });
            }
    
            CurrentProgressNormalized = progressNormalized;
        }
    
        protected virtual void OnFillStart() { }
        protected virtual void OnFillEnd() { }
    
        private Vector2 GetBarScaleForProgress(float progressNormalized)
        {
            return new Vector2(Mathf.Clamp(progressNormalized, 0f, 1f) * fullBarScale, bar.transform.localScale.y);
        }
    }
    
}
