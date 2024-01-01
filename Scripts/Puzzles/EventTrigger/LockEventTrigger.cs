using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockEventTrigger : MonoBehaviour
{
    private bool _activated = false;
    public void OnTriggerEnter(Collider other)
    {
        if (!_activated && other.gameObject.layer == LayerMask.NameToLayer("Creature") && !DialogueManagerScript.Instance.InProgress)
        {
            DialogueManagerScript.Instance.Event9();
            _activated = true;
        }
    }
}
