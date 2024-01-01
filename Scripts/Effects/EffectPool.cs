using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour, IObjectPool<PooledEffect>
{
    private IObjectPool<PooledEffect> _pool;
    private PooledEffect _shaderParticleEffects;
    
    public PooledEffect Get() => _pool.Get();
    public PooledObject<PooledEffect> Get(out PooledEffect v) => _pool.Get(out v);
    public void Release(PooledEffect element) => _pool.Release(element);
    public void Clear() => _pool.Clear();
    public int CountInactive => _pool.CountInactive;
    public void Init(PooledEffect shaderParticleEffects) 
    {
        _shaderParticleEffects = shaderParticleEffects;
        int maxEffects = shaderParticleEffects.MaxEffects != 0 ? shaderParticleEffects.MaxEffects : 100;
        _pool = new ObjectPool<PooledEffect>(CreatePooledEffect, OnPoolGet, OnPoolRelease, OnPoolDestroy, 
            true, 10, maxEffects); 
    }
    private PooledEffect CreatePooledEffect() 
    {
        var effect = Instantiate(_shaderParticleEffects, transform);
        effect.Init(_pool);
        return effect;
    }
    private static void OnPoolRelease(PooledEffect effect) => effect.gameObject.SetActive(false);
    private static void OnPoolGet(PooledEffect effect) => effect.gameObject.SetActive(true);
    private static void OnPoolDestroy(PooledEffect effect) => Destroy(effect.gameObject);
}
