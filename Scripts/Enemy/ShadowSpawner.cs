using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowSpawner : MonoBehaviour
{
    private const byte MaxShadows = 20;
    private static byte _shadowAmount;

    public bool ShouldSpawn = false;

    public static byte ShadowAmount
    {
        get => _shadowAmount;
        set => _shadowAmount = (byte)Mathf.Clamp(value, 0, MaxShadows);
    }

    [Min(0)][SerializeField] private float _spawnRateMin = 3f, _spawnRateMax = 5f;
    [SerializeField] private GameObject _shadowPrefab;
    
    private void OnEnable() => StartCoroutine(SpawnLoop());

    private void OnDisable() => StopAllCoroutines();

    IEnumerator SpawnLoop()
    {
        do
        {
            float spawnTime = Random.Range(_spawnRateMin, _spawnRateMax);

            yield return new WaitForSeconds(spawnTime);

            if (ShadowAmount >= MaxShadows || !ShouldSpawn) continue;
            ShadowAmount++;
            
            SpawnShadow();    
        } while (true);
    }

    void SpawnShadow()
    {
        EffectsManager.Instance.SpawnParticleEffect("ShadowSpawn", transform.position, Quaternion.identity);
        AudioManager.Instance.PlayAmbientWorld("ShadowSpawn", transform.position, 30f);
        Instantiate(_shadowPrefab, transform.position, Quaternion.identity);
    }

}
