using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : ManagerInstance<AudioManager>
{
	public static AudioMixer CurrentMixer;
	
	private Dictionary<string, SoundList.AudioSample> _storedSounds = new();
	private AudioPool _sfxPool, _ambientPool;

	private void Awake()
	{
		CurrentMixer = Resources.Load<AudioMixer>("AudioMixer");
		
		var sfxPool = new GameObject($"SoundPool").AddComponent<AudioPool>();
		sfxPool.Init("Effects");
		_sfxPool = sfxPool;
		
		var ambientPool = new GameObject($"Ambient").AddComponent<AudioPool>();
		ambientPool.Init("Ambient");
		_ambientPool = ambientPool;
		
		SoundList sounds = Resources.Load<SoundList>("SoundList");
		foreach (var sound in sounds.Sounds) {
			_storedSounds.Add(sound.KeyName, sound);
		}
	}

	public void PlaySoundWorld(string key, Vector3 position, float radius = 5f, float playDelay = 0, float pitch = 1f)
	{
		if(!_storedSounds.ContainsKey(key)) return;
		var audioSample = _storedSounds[key];

		var pool = _sfxPool.Get();
		pool.PlaySoundWorld(audioSample, position, radius, pitch, playDelay);
	}

	public void PlaySound(string key, float playDelay = 0, float pitch = 1f, float panning = 0f)
	{
		if(!_storedSounds.ContainsKey(key)) return;
		var audioSample = _storedSounds[key];

		var pool = _sfxPool.Get();
		pool.PlaySound(audioSample, pitch, playDelay, panning);
	}

	public void PlayAmbientWorld(string key, Vector3 position, float radius = 5f, float playDelay = 0, float pitch = 1f)
	{
		if(!_storedSounds.ContainsKey(key)) return;
		var audioSample = _storedSounds[key];

		var pool = _ambientPool.Get();
		pool.PlaySoundWorld(audioSample, position, radius, pitch, playDelay);
	}
	
	public void PlayAmbient(string key, float playDelay = 0, float pitch = 1f, float panning = 0f)
	{
		if(!_storedSounds.ContainsKey(key)) return;
		var audioSample = _storedSounds[key];

		var pool = _ambientPool.Get();
		pool.PlaySound(audioSample, pitch, playDelay, panning);
	}
}