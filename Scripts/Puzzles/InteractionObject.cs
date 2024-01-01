using System;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    public enum PlayerType {smallPlayer, bigPlayer, both}

    public PlayerType TargetPlayer;

    public byte PlayerIndex => (byte)(TargetPlayer == PlayerType.smallPlayer ? 1 : 0);
    
    public bool playerInRange;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!CheckPlayer(other)) return;
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!CheckPlayer(other)) return;
        playerInRange = false;
    }

    private bool CheckPlayer(Collider other)
    {
        switch (TargetPlayer)
        {
            case PlayerType.smallPlayer:
                return other.transform.GetComponent<PlayerController>();
            case PlayerType.bigPlayer:
                return other.transform.GetComponent<BeastPlayerController>();
            case PlayerType.both:
                return other.transform.GetComponent<PlayerController>();
        }
        return false;
    }
}