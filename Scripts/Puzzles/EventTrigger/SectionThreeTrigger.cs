using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionThreeTrigger : MonoBehaviour
{
    private bool _activated;

    public void OnTriggerEnter(Collider other)
    {
        if (!_activated && other.gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            DialogueManagerScript.Instance.Event10();

            foreach (var spawners in FindObjectsOfType<ShadowSpawner>()) //ugly fix pls remove later
            {
                spawners.ShouldSpawn = true;
            }

            _activated = true;
        }
    }

}
