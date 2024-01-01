using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarTriggerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BeastPlayerController>())
        {
            transform.parent.GetComponent<PillarBehaviour>().PlayerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<BeastPlayerController>())
        {
            transform.parent.GetComponent<PillarBehaviour>().PlayerInRange = false;
            other.gameObject.GetComponent<BeastPlayerController>().Paralyzed = false; // dirty fix
        }
    }
}
