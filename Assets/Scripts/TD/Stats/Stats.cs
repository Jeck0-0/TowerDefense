using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;


namespace TowerDefense
{
    public class Stats
    {
        [ShowInInspector]
        public Dictionary<string, Stat> stats {  get; private set; }
    
        public Stat this[string s] => stats[s];
        public Stats()
        {
            stats = new Dictionary<string, Stat>();
        }
    
        public void AddStat(string name, Stat stat)
        {
            if (stats.ContainsKey(name))
                Debug.LogError("duplicate stat: " + name + ": " + stat);
            
            stat.SetName(name);
            stats.Add(name, stat);
        }
        public void AddStat(string name, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            if (stats.ContainsKey(name))
                Debug.LogError("duplicate stat: " + name + ": " + baseValue);
    
            AddStat(name, new Stat(name, baseValue, minValue, maxValue));
        }
        
        public bool HasStat(string name) => stats.ContainsKey(name);
    
        public void AddModifier(string statName, string modifierName, float add = 0, float multiply = 1, bool overrideIfDuplicate = true)
            => stats[statName].AddModifier(modifierName, add, multiply, overrideIfDuplicate);
    
        public bool HasModifier(string statName, string modifierName)
            => stats[statName].HasModifier(modifierName);
    
        public void RemoveModifier(string statName, string modifierName)
            => stats[statName].RemoveModifier(modifierName);
    }
    
    
    [Serializable, InlineProperty, BoxGroup("Stats"), LabelWidth(100)]
    public class Stat
    {
        [SerializeField, HideInInspector]
        private string m_name;
        [HideInInspector]
        public string Name { get; protected set; }
    
    
        [SerializeField, HideLabel, HorizontalGroup("Values")]
        [OnValueChanged("@UpdateValue()")]
        private float m_baseValue;
        public float BaseValue { get => m_baseValue; protected set => m_baseValue = value; }
    
    
        private float m_value;
        [ShowInInspector, ReadOnly, HorizontalGroup("Values"), LabelText("Current"), LabelWidth(48)]
        public float Value { 
            get { if (!isInitialized) Initialize(); 
                return m_value; } 
            private set => m_value = value; 
        }
    
        #region RangeEditor
    
        [HorizontalGroup("Range")]
        [HorizontalGroup("Range/Min", width: 60, LabelWidth = 30)]
        [Tooltip("@min?\"Disable minimum value\":\"Enable minimum value\"")]
        [Button("@min?\"Min On\":\"Min Off\"")]
        public void ToggleMin()
        {
            min = !min;
            if (min) MinValue = tempMin;
            else { tempMin = MinValue; MinValue = float.MinValue; }
            UpdateValue();
        }
        [HorizontalGroup("Range")]
        [HorizontalGroup("Range/Max", width: 60, LabelWidth = 30)]
        [Tooltip("@max?\"Disable maximum value\":\"Enable maximum value\"")]
        [Button("@max?\"Max On\":\"Max Off\"")]
        protected void ToggleMax()
        {
            max = !max;
            if (max) MaxValue = tempMax;
            else { tempMax = MaxValue; MaxValue = float.MaxValue; }
            UpdateValue();
        }
        [SerializeField, HideInInspector]
        protected bool min;
        [SerializeField, HideInInspector]
        protected bool max;
        [SerializeField, HideInInspector]
        protected float tempMin;
        [SerializeField, HideInInspector]
        protected float tempMax;
    
        #endregion 
    
        [SerializeField]
        [HorizontalGroup("Range"), EnableIf("@min"), HideLabel]
        [HorizontalGroup("Range/Min")]
        [OnValueChanged("@UpdateValue()")]
        protected float m_minValue;
        public float MinValue { get => m_minValue; private set => m_minValue = value; }
    
        [SerializeField]
        [HorizontalGroup("Range"), EnableIf("@max"), HideLabel]
        [HorizontalGroup("Range/Max")]
        [OnValueChanged("@UpdateValue()")]
        private float m_maxValue;
        public float MaxValue { get => m_maxValue; private set => m_maxValue = value; }
    
    
        [ShowInInspector]
        [OnValueChanged("@UpdateValue()")]
        public Dictionary<string, StatModifier> modifiers { get; private set; }
    
        public event Action<StatValueChangedEventArgs> OnValueChanged;
    
        public static implicit operator float(Stat s) => s.Value;
    
        private bool isInitialized;
            
        public Stat()
        {
            modifiers = new Dictionary<string, StatModifier>();
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
        }
    
        public Stat(string name, float baseValue, float minValue, float maxValue) : base()
        {
            this.Name = name;
            this.BaseValue = baseValue;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
                
            modifiers = new Dictionary<string, StatModifier>();
            UpdateValue();
        }
    
        protected void Initialize()
        {
            isInitialized = true;
            min = MinValue != float.MinValue;
            max = MaxValue != float.MaxValue;
            UpdateValue();
        }
    
        public void SetName(string newName) => Name = newName;
        public void SetBaseValue(float newValue)
        {
            BaseValue = newValue;
            UpdateValue();
        }
    
        public void AddModifier(string id, float add = 0, float multiply = 1, bool overrideIfDuplicate = true)
        {
            //override existing
            if (modifiers.ContainsKey(id) && overrideIfDuplicate)
            {
                modifiers[id].multiply = multiply;
                modifiers[id].add = add;
            }
            //create new (regardless of override)
            else if(!modifiers.ContainsKey(id))
            {
                modifiers[id] = new StatModifier(add, multiply);
            }
            //no override (sum)
            else
            {
                modifiers[id].add += add;
                modifiers[id].multiply += multiply - 1;
            }
            
            UpdateValue();
        }
    
        public bool HasModifier(string id)
        {
            return modifiers.ContainsKey(id);
        }
        public void RemoveModifier(string id)
        {
            if (modifiers.ContainsKey(id))
                modifiers.Remove(id);
            UpdateValue();
        }
    
    
        public static float GetValueWithModifiers(Stat stat, IEnumerable<StatModifier> mods) => GetValueWithModifiers(stat.BaseValue, mods, stat.MinValue, stat.MaxValue);
        public static float GetValueWithModifiers(float baseValue, IEnumerable<StatModifier> mods, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            float add = mods.Sum(x => x.add);
            float multiply = 1 + mods.Sum(x => x.multiply - 1);
            float res = baseValue * multiply + add;
    
            return Mathf.Clamp(res, minValue, maxValue);
        }
    
        protected void UpdateValue()
        {
            float previousValue = Value;
            Value = GetValueWithModifiers(this, modifiers.Values);
    
            if (previousValue != Value)
            {
                OnValueChanged?.Invoke(new StatValueChangedEventArgs(previousValue, Value));
            }
        }
    
        public class StatValueChangedEventArgs
        {
            public float previousValue;
            public float newValue;
            public StatValueChangedEventArgs(float previousValue, float newValue)
            {
                this.previousValue = previousValue;
                this.newValue = newValue;
            }
        }
    
        [Serializable]
        public class StatModifier
        {
            public float add;
            public float multiply = 1;
            public StatModifier() { }
            public StatModifier(float add, float multiply)
            {
                this.add = add;
                this.multiply = multiply;
            }
        }
    }
}
