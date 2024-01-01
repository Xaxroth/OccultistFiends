using UnityEngine;
using UnityEngine.Pool;

public class AudioPool : MonoBehaviour, IObjectPool<PooledAudio>
{
    private IObjectPool<PooledAudio> _pool;
    private string _audioGroup;

    public PooledAudio Get() => _pool.Get();
    public PooledObject<PooledAudio> Get(out PooledAudio v) => _pool.Get(out v);
    public void Release(PooledAudio element) => _pool.Release(element);
    public void Clear() => _pool.Clear();
    public int CountInactive => _pool.CountInactive;
    public void Init(string audioGroup)
    {
        _audioGroup = audioGroup;
        _pool = new ObjectPool<PooledAudio>(CreatePooledAudio, OnPoolGet, OnPoolRelease, OnPoolDestroy, 
            true, 5, 20); 
    }
    private PooledAudio CreatePooledAudio()
    {
        var audioPool = new GameObject("AudioPool");
        audioPool.AddComponent<PooledAudio>();
        audioPool.transform.parent = transform;
        var script = audioPool.GetComponent<PooledAudio>();
        
        script.Init(_pool, _audioGroup);
        return script;
    }
    private static void OnPoolRelease(PooledAudio pool) => pool.gameObject.SetActive(false);
    private static void OnPoolGet(PooledAudio pool) => pool.gameObject.SetActive(true);
    private static void OnPoolDestroy(PooledAudio pool) => Destroy(pool.gameObject);
}
