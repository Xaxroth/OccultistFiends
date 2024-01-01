using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cam;

    void Start()
    {
        _cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(_cam.position);
    }
}
