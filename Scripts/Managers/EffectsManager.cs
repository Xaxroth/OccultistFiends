using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : ManagerInstance<EffectsManager>
{
    private Dictionary<string, PooledEffect> _storedEffects = new();
    private Dictionary<string, EffectPool> _activeEffects = new();
    private void Awake() 
    {
        EffectsList effects = Resources.Load<EffectsList>("EffectList");
        foreach (var effect in effects.Effects) {
            if(!effect.Prefab.TryGetComponent(out PooledEffect pooledEffect)) continue;
            _storedEffects.Add(effect.KeyName, pooledEffect);
        }
    }

    public void SpawnParticleEffect(string key, Vector3 position, Quaternion rotation, float playDelay = 0) 
    {
        if (!_activeEffects.ContainsKey(key)) 
        {
            if(!_storedEffects.ContainsKey(key)) return;
            var effect = _storedEffects[key];

            var newPool = new GameObject($"EffectPool: {key}").AddComponent<EffectPool>();
            newPool.Init(effect);
        
            _activeEffects.Add(key, newPool);
        }

        PooledEffect particleEffect = _activeEffects[key].Get();
        particleEffect.SetTransform(position, rotation);
        particleEffect.PlayDelay(playDelay);
    }
}
