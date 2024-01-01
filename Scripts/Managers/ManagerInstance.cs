using UnityEngine;

[DefaultExecutionOrder(-1)]
public abstract class ManagerInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            _instance ??= FindObjectOfType<T>();

            if (_instance != null) return _instance;

            GameObject manager = new GameObject(typeof(T).Name);
            _instance = manager.AddComponent<T>();

            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null) return;
        _instance = gameObject.GetComponent<T>();
    }
}
