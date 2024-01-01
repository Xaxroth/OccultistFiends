using UnityEngine;
using UnityEngine.Pool;

public class PooledAudio : MonoBehaviour
{
    private AudioSource _audioSource;
    private IObjectPool<PooledAudio> _pool;

    private bool _playing;
    private Transform _cam;
    
    public void Init(IObjectPool<PooledAudio> pool, string audioGroup)
    {
        _pool = pool;
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.outputAudioMixerGroup = AudioManager.CurrentMixer.FindMatchingGroups(audioGroup)[0];
        _audioSource.playOnAwake = false;

        _cam = Camera.main.transform;
    }

    private void Update()
    {
        if (_audioSource.isPlaying || !_playing) return;
        _pool.Release(this);
        _playing = false;
    }

    public void PlaySound(SoundList.AudioSample sample, float pitch = 1f, float delay = 0f, float panning = 0f)
    {
        _audioSource.spatialBlend = 0f;
        _audioSource.reverbZoneMix = 0f;
        _audioSource.clip = sample.Clip;
        _audioSource.volume = sample.Volume;
        _audioSource.pitch = pitch;
        _audioSource.minDistance = 250;
        _audioSource.panStereo = panning;

        transform.position = _cam.position;
        
        Invoke(nameof(Play), delay);
    }

    public void PlaySoundWorld(SoundList.AudioSample sample, Vector3 pos, float radius, float pitch = 1f, float delay = 0f)
    {
        _audioSource.spatialBlend = 1f;
        _audioSource.reverbZoneMix = 1f;
        _audioSource.clip = sample.Clip;
        _audioSource.volume = sample.Volume;
        _audioSource.pitch = pitch;
        _audioSource.minDistance = radius;
        _audioSource.panStereo = 0f;

        transform.position = pos;
        
        Invoke(nameof(Play), delay);
    }

    void Play()
    {
        _audioSource.Play();
        _playing = true;
    }
}
