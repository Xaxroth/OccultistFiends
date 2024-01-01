using UnityEngine;

[CreateAssetMenu(menuName = "Lists/SoundList")]
public class SoundList : ScriptableObject
{
    [System.Serializable]
    public struct AudioSample
    {
        public string KeyName;
        public AudioClip Clip;
        [Range(0, 1)] public float Volume;
    }

    [SerializeField] 
    public AudioSample[] Sounds;
}
