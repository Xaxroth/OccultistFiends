using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBarrierBehaviour : MonoBehaviour
{
	private PillarScript _pillar;

    [SerializeField]
    private GameObject _barrier;

	private void Awake()
	{
       _pillar = GetComponent<PillarScript>();
	}



	void Update()
    {
        if (_pillar._pillarDestroyed == true)
        {
            Destroy(_barrier);
        }

}
}
