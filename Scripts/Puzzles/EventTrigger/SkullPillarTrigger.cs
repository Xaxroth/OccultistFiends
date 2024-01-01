using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullPillarTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _trigger;
    private bool _activated = false;
    public void OnTriggerEnter(Collider other)
    {
        if (!_activated && other.gameObject.layer == LayerMask.NameToLayer("Cultist") && !DialogueManagerScript.Instance.InProgress)
        {
            DialogueManagerScript.Instance.Event7();
            _activated = true;
            _trigger.SetActive(false);
        }
    }
}
