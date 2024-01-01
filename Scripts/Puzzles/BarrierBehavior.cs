using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierBehavior : MonoBehaviour
{
    void Update()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, PlayerController.Instance.gameObject.layer, !PlayerController.Instance.BeingThrown);

        Physics.IgnoreLayerCollision(gameObject.layer, BeastPlayerController.Instance.gameObject.layer, !PlayerController.Instance.BeingHeld);
    }
}
