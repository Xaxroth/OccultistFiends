using UnityEngine;

[CreateAssetMenu(menuName = "Lists/EffectList")]
public class EffectsList : ScriptableObject
{
    [System.Serializable]
    public struct Effect
    {
        public GameObject Prefab;
        public string KeyName;
    }

    [SerializeField]
    public Effect[] Effects;
}
