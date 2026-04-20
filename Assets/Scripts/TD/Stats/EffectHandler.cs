using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TowerDefense
{
    public class EffectHandler : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        public Dictionary<string, ActiveEffect> effects = new();

        public Action<Effect> OnEffectAdded;
        public Action<Effect> OnEffectRemoved;
    
    
        public void AddEffect(Effect effect, float duration, bool forceUpdateDuration = false)
        {
            if (effects.ContainsKey(effect.Name)) 
            {
                if (effects[effect.Name].activeUntil < Time.time + duration || forceUpdateDuration)
                    effects[effect.Name].activeUntil = Time.time + duration;
                return;
            }
    
            effects.Add(effect.Name, new ActiveEffect(effect, Time.time + duration));
            effect.handler = this;
            effect.StartEffect();
            OnEffectAdded?.Invoke(effect);
        }
    
        public bool HasEffect(string name)
        {
            return effects.ContainsKey(name);
        }

        public void RemoveEffect(string name)
        {
            if (!effects.ContainsKey(name))
                return;
            effects[name].EndEffect();
            OnEffectRemoved?.Invoke(effects[name].effect);
            effects.Remove(name);
        }
    
        float nextTick;
        private void Update()
        {
            if (Time.time < nextTick) return;
            nextTick = Time.time + (1f / GameManager.gameTickFrequency);
    
            List<string> toRemove = new List<string>();
            foreach (var effect in effects)
            {
                if (effect.Value.activeUntil < Time.time)
                {
                    toRemove.Add(effect.Key);
                    continue;
                }
    
                effect.Value.UpdateEffect();
            }
    
            //avoids changing effects list while iterating it
            foreach (var effect in toRemove)
                RemoveEffect(effect);
        }
    
        public class ActiveEffect
        {
            public float activeUntil;
            public Effect effect;
    
            public void StartEffect() => effect.StartEffect();
            public void UpdateEffect() => effect.UpdateEffect();
            public void EndEffect() => effect.EndEffect();
    
            public ActiveEffect(Effect effect, float duration) 
            {
                this.effect = effect;
                this.activeUntil = duration;
            }
        }
    }
    
    
    public abstract class Effect
    {
        public string Name => name;
        private string name;
        public EffectType Type => type;
        EffectType type;

        public EffectHandler handler;
        
        public Effect(string name, EffectType type) 
        { 
            this.name = name; 
            this.type = type;
        }
        
        public virtual void StartEffect() { }
        public virtual void UpdateEffect() { }
        public virtual void EndEffect() { }
    }
    public enum EffectType { stun, slow, fire, special }
}
