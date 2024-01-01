using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTrigger : MonoBehaviour
{
    private bool _activated = false;
    public void OnTriggerEnter(Collider other)
    {
        if (!_activated && other.gameObject.layer == LayerMask.NameToLayer("Creature") && !DialogueManagerScript.Instance.InProgress)
        {
            DialogueManagerScript.Instance.Event12();
            _activated = true;
        }
    }
}
