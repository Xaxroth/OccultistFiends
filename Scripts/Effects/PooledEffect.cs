using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

public class PooledEffect : MonoBehaviour
{
    private enum ParticleSystemType{ParticleSystem, VisualEffectsGraph}

    private ParticleSystemType _type;
    [Tooltip("Manual lifetime, necessary for vfx graph. 0")]
    [SerializeField, Min(0)] private float _lifeTime = 0;
    [Tooltip("The max instances that can be in the scene. 0 defaults to 100")]
    [SerializeField, Min(0)] private int _maxEffects = 0;

    private IObjectPool<PooledEffect> _pool;
    private ParticleSystem _particleSystem;
    private VisualEffect _visualEffect;
    
    public int MaxEffects => _maxEffects;

    private void OnParticleSystemStopped() => _pool.Release(this);

    public void Init(IObjectPool<PooledEffect> objectPool) 
    {
        _pool = objectPool;
        if (TryGetComponent(out _particleSystem)) 
        {
            var main = _particleSystem.main;
            if(_lifeTime == 0)main.stopAction = ParticleSystemStopAction.Callback;
            _type = ParticleSystemType.ParticleSystem;
        }
        else if (TryGetComponent(out _visualEffect)) 
        {
            _type = ParticleSystemType.VisualEffectsGraph;
        }
    }
    
    public void SetTransform(Vector3 position, Quaternion rotation) => transform.SetPositionAndRotation(position, rotation);
    public void PlayDelay(float delay) => Invoke(nameof(Play), delay);
    private void Play() 
    {
        if(_particleSystem == null && _visualEffect == null)
            return;
        
        switch (_type) 
        {
            case ParticleSystemType.ParticleSystem:
                _particleSystem.Play();
                if(_lifeTime != 0) Invoke(nameof(OnParticleSystemStopped), _lifeTime);
                break;
            case ParticleSystemType.VisualEffectsGraph:
                _visualEffect.Play();
                var time = _lifeTime != 0 ? _lifeTime : 100f;
                Invoke(nameof(OnParticleSystemStopped), time); 
                break;
        }
    }
}
